using CorpusLegis.API.Data;
using CorpusLegis.API.Domain;
using CorpusLegis.API.Exceptions;
using CorpusLegis.Contracts.PdfGeneration;
using CorpusLegis.Contracts.ReputatioCalculus;
using CorpusLegis.Shared.Dtos;
using CorpusLegis.Shared.Enums;
using FluentValidation;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System.Reflection.Metadata.Ecma335;

namespace CorpusLegis.API.Services;

public class RogatioService : IRogatioService
{
    private readonly CorpusLegisContext _db;
    private readonly ICurrentUserService _currentUser;

    private readonly IValidator<CreateRogatioDto> _createValidator; // TODO: el validador se ha llevado al Endpoint, que es donde debe estar. Quita esto.

    private readonly IPublishEndpoint _publishEndpoint;

    private readonly ILogger<RogatioService> _logger;

    //Guid CivisDefaultGuid = Guid.Parse("0f8fad5b-d9cb-469f-a165-70867728950e"); // Sempronio
    Guid CivitasDefaultGuid = Guid.Parse("7c9e6679-7425-40de-944b-e07fc1f90ae7"); // Solfamidas

    public RogatioService(CorpusLegisContext db, ICurrentUserService currentUser, IValidator<CreateRogatioDto> createValidator, IPublishEndpoint publishEndpoint, ILogger<RogatioService> logger)
    {
        _db = db;
        _currentUser = currentUser;
        _createValidator = createValidator;
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }



    public async Task<List<RogatioSummaryDto>> GetAllAsync()
    {
        var currentCivisId = _currentUser.CivisId;

        return await _db.Rogationes
                .AsNoTracking()
                .Where(r => _db.CivitasSodales.Any(cs => cs.CivisId == currentCivisId && cs.CivitasId == r.CivitasId))
                .Select(r => new RogatioSummaryDto (
                    r.Id,
                    r.Title,
                    r.Content,
                    r.CivisId,
                    r.Civitas.Name,
                    r.CreatedAt,
                    r.Status,
                    r.CivisId == currentCivisId && r.Status == RogatioStatus.Inchoatus, // puede editar.
                    r.CivisId == currentCivisId && r.Status == RogatioStatus.Inchoatus // puede eliminar.
                    ))
                .ToListAsync();
    }

    public async Task<RogatioDetailsDto?> GetByIdAsync(Guid id)
    {
        var currentCivisId = _currentUser.CivisId;

        var rogatioInfo = await _db.Rogationes
                            .AsNoTracking()
                            .Where(r => r.Id == id)
                            .Select(r => new
                            {
                                Dto = new RogatioDetailsDto
                                 (
                                    r.Id,
                                    r.Title,
                                    r.Content,
                                    r.CivisId,
                                    r.Civis.Name,
                                    r.CivitasId,
                                    r.Civitas.Name,
                                    r.CreatedAt,
                                    r.Status,
                                    r.Suffragia.Count(s => s.Votum == SuffragiumValue.Pro),
                                    r.Suffragia.Count(s => s.Votum == SuffragiumValue.Contra),
                                    r.Suffragia.Count(s => s.Votum == SuffragiumValue.Abstentio),
                                    r.Suffragia.Any(s => s.CivisId == currentCivisId),
                                    r.RequiredQuorum,
                                    r.RequiredMajority
                                ),
                                IsMember = _db.CivitasSodales.Any(cs => cs.CivisId == currentCivisId && cs.CivitasId == r.CivitasId)
                            })
                            .FirstOrDefaultAsync();

        if (rogatioInfo == null)
        {
            throw new NotFoundException("Rogatio", id);
        }

        if (!rogatioInfo.IsMember)
        {
            throw new UnauthorizedDomainException("No formas parte de la Civitas a la que pertenece esta Rogatio."); // TODO: escrutinio-worker pasa por aquí. Acaba haciendo su evaluación pero la API lanza esta excepción porque el microservicio no pertenece a la Civitas. Hay que controlar esto.
        }

        return rogatioInfo.Dto;
    }

    public async Task<RogatioDetailsDto> CreateAsync(CreateRogatioDto newRogatio)
    {
        var currentCivisId = _currentUser.CivisId;

        var civitasInfo = await _db.Civitates
            .Where(c => c.Id == newRogatio.CivitasId)
            .Select(c => new
            {
                Exist = true,
                IsMember = c.Cives.Any(civis => civis.Id == currentCivisId)
            })
            .FirstOrDefaultAsync();

        if (civitasInfo == null)
        {
            throw new BusinessRuleValidationException($"No existe una Civitas con el ID {newRogatio.CivitasId}.");
        }

        if (!civitasInfo.IsMember)
        {
            throw new UnauthorizedDomainException($"El Civis con ID {currentCivisId} no pertenece a la Civitas con ID {newRogatio.CivitasId}. Por lo tanto, no puede crear una Rogatio para ella.");
        }

        Rogatio rogatio = new Rogatio
        {
            Id = Guid.NewGuid(),
            Title = newRogatio.Title,
            Content = newRogatio.Content,
            CivisId = currentCivisId,
            CivitasId = newRogatio.CivitasId,
            CreatedAt = DateTime.UtcNow,
            Status = newRogatio.Status,
            //Deadline = newRogatio.Deadline, // TODO: vuelve a descomentar esto para tener la fecha en futuro y no en pasado ++ añadir a la UI.
            RequiredQuorum = newRogatio.RequiredQuorum, // TODO: añadir a la UI.
            RequiredMajority = newRogatio.RequiredMajority // TODO: añadir a la UI.

        };

        _db.Rogationes.Add(rogatio);

        var evento = new RogatioCreatedIntegrationEvent
        {
            CivisId = currentCivisId,
            RogatioId = rogatio.Id,
            Timestamp = DateTime.UtcNow
        };
        await _publishEndpoint.Publish(evento);

        await _db.SaveChangesAsync();

        return await GetByIdAsync(rogatio.Id);
    }

    public async Task<RogatioDetailsDto?> UpdateAsync(Guid id, UpdateRogatioDto updatedRogatio)
    {
        var currentCivisId = _currentUser.CivisId;

        var existingRogatio = await _db.Rogationes.FindAsync(id);

        if (existingRogatio == null)
        {
            return null;
        }

        if (existingRogatio.CivisId != currentCivisId)
        {
            throw new UnauthorizedDomainException("Sólo el creador de la Rogatio puede editarla.");
        }

        if (existingRogatio.Status != RogatioStatus.Inchoatus)
        {
            throw new InvalidOperationException($"No está permitido editar una Rogatio que no está en estado Inchoatus. Estado actual: {existingRogatio.Status}");
        }

        existingRogatio.Title = updatedRogatio.Title;
        existingRogatio.Content = updatedRogatio.Content;
        existingRogatio.CivisId = currentCivisId;
        existingRogatio.CivitasId = existingRogatio.CivitasId; // la Civitas nunca puede cambiar.
        existingRogatio.Status = updatedRogatio.Status;

        await _db.SaveChangesAsync();

        return await GetByIdAsync(id);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var currentCivisId = _currentUser.CivisId;

        var rogatioMeta = await _db.Rogationes
            .Where(r => r.Id == id)
            .Select(r => new { r.CivisId, r.Status }) // me traigo sólo los campos que me interesa validar, para hacer más ligera la consulta.
            .FirstOrDefaultAsync();

        if (rogatioMeta == null)
        {
            return false;
        }

        if (rogatioMeta.CivisId != currentCivisId)
        {
            throw new UnauthorizedDomainException("Sólo el creador de la Rogatio puede editarla.");
        }

        if (rogatioMeta.Status != RogatioStatus.Inchoatus)
        {
            throw new InvalidOperationException($"Sólo se pueden borrar Rogatios en estado de Inchoatus. Estado actual: {rogatioMeta.Status}");
        }

        var filasBorradas =  await _db.Rogationes.Where(r => r.Id == id)
                                                .ExecuteDeleteAsync();

        return filasBorradas > 0;
    }




    public async Task<RogatioDetailsDto?> ChangeStatusAsync(Guid id, WorkflowRogatioDto workflowRogatio)
    {
        var currentCivisId = _currentUser.CivisId;

        var rogatio = await _db.Rogationes.FindAsync(id);

        if (rogatio == null)
        {
            throw new NotFoundException("Rogatio", id);
        }

        var estadoActual = rogatio.Status;
        RogatioStatus nuevoEstado = workflowRogatio.Status;

        if (rogatio.CivisId != currentCivisId)
        {
            throw new UnauthorizedDomainException("Sólo el creador de la Rogatio puede progresarla.");
        }

        bool transicionValida = (estadoActual, nuevoEstado) switch
        {
            (RogatioStatus.Inchoatus, RogatioStatus.Proposita) => true,
            (RogatioStatus.Proposita, RogatioStatus.Inchoatus) => true,
            (RogatioStatus.Proposita, RogatioStatus.InSuffragium) => true,
            (RogatioStatus.InSuffragium, RogatioStatus.Approbata) => true,
            (RogatioStatus.InSuffragium, RogatioStatus.Reprobata) => true,
            _ => false
        };


        if (!transicionValida) {
            // TODO: aquí habría que lanzar una DomainException personalizada, indicando que la transición no es válida.
            throw new Exception($"Transición no válida: {estadoActual} -> {nuevoEstado}.");
        }

        // TODO: esto es temporal hasta que termino de crear toda la parafernalia.
        if (nuevoEstado == RogatioStatus.Approbata)
        {
            var nuevaLex = new Lex
            {
                Id = Guid.NewGuid(),
                CivitasId = rogatio.CivitasId,
                OriginRogatioId = rogatio.Id,
                Title = rogatio.Title,
                Content = rogatio.Content,
                PromulgatedAt = DateTime.UtcNow
            };
            _db.Leges.Add(nuevaLex);
        }
        // TODO: hasta aquí la temporalidad.

        rogatio.Status = nuevoEstado;

        await _db.SaveChangesAsync();

        return await GetByIdAsync(id);

    }



    public async Task<RogatioDetailsDto?> EvalueAsync(Guid id, WorkflowRogatioDto workflowRogatio)
    {
        var escrutinioData = await _db.Rogationes
            .Where(r => r.Id == id)
            .Select(r => new
            {
                Rogatio = r,
                PoblacionCivitas = r.Civitas.Cives.Count(),
                VotosTotales = r.Suffragia.Count(),
                VotosPro = r.Suffragia.Count(s => s.Votum == SuffragiumValue.Pro)
            })
            .FirstOrDefaultAsync();

        if (escrutinioData == null)
        {
            throw new NotFoundException("Rogatio", id);
        }

        var rogatio = escrutinioData.Rogatio;

        if (rogatio.Status == RogatioStatus.Approbata || rogatio.Status == RogatioStatus.Reprobata) // El ciclo de vida de la Rogatio ya ha acabado.
        {
            return await GetByIdAsync(id);
        }

        if (rogatio.Status == RogatioStatus.Inchoatus || rogatio.Status == RogatioStatus.Proposita) // Aún no ha llegado el momento de decidir qué pasa con la Rogatio.
        {
            return await GetByIdAsync(id);
        }

        if (DateTime.UtcNow < rogatio.Deadline)
        {
            throw new BusinessRuleValidationException("Aún no se ha cumplido la fecha límite de votación de la Rogatio, por lo que no se puede resolver.");
        }

        if (escrutinioData.VotosTotales == 0)
        {
            rogatio.Status = RogatioStatus.Reprobata;
            await _db.SaveChangesAsync();
            return await GetByIdAsync(id);
        }

        var requiredQuorumCount = (int)Math.Ceiling(escrutinioData.PoblacionCivitas * (double)rogatio.RequiredQuorum);

        if (escrutinioData.VotosTotales < requiredQuorumCount) // no ha habido cuórum.
        {
            rogatio.Status = RogatioStatus.Reprobata;
            await _db.SaveChangesAsync();
            return await GetByIdAsync(id);
        }

        decimal proFraction = (decimal)escrutinioData.VotosPro / (decimal)escrutinioData.VotosTotales;

        if (proFraction >= rogatio.RequiredMajority) // sí ha habido mayoría.
        {
            var nuevaLex = new Lex
            {
                Id = Guid.NewGuid(),
                CivitasId = rogatio.CivitasId,
                OriginRogatioId = rogatio.Id,
                Title = rogatio.Title,
                Content = rogatio.Content,
                PromulgatedAt = DateTime.UtcNow
            };

            _db.Leges.Add(nuevaLex);
            rogatio.Status = RogatioStatus.Approbata;

            await _publishEndpoint.Publish(new LexPromulgatedIntegrationEvent
            {
                LexId = nuevaLex.Id,
                Title = nuevaLex.Title,
                Content = nuevaLex.Content,
                PromulgatedAt = nuevaLex.PromulgatedAt
            });

            await _db.SaveChangesAsync();
        }
        else
        {
            rogatio.Status = RogatioStatus.Reprobata;
            await _db.SaveChangesAsync();
        }

        return await GetByIdAsync(id);

    }

    public async Task<int> EvaluatePendingAsync()
    {
        var pendingRogationes = await _db.Rogationes
            .Where(r => r.Status == RogatioStatus.InSuffragium && r.Deadline <= DateTime.UtcNow)
            .Select(r => r.Id)
            .ToListAsync();

        int processedCount = 0;

        foreach (var rogatioId in pendingRogationes)
        {
            try
            {
                await EvalueAsync(rogatioId, new WorkflowRogatioDto { Id = rogatioId, Status = RogatioStatus.InSuffragium});
                processedCount++;
            }
            catch (Exception ex)
            {
                // TODO: integrar ILogger<RogatioService> cuando lo tenga.
                Console.WriteLine($"Error al evaluar la Rogatio con ID {rogatioId}: {ex.Message}");
                _logger.LogError(ex, "Error al evaluar la Rogatio {RogatioId}", rogatioId);
            }
        }

        return processedCount;

    }



}
