var builder = DistributedApplication.CreateBuilder(args);

// Message Broker
var rabbitmq_corpuslegis = builder.AddRabbitMQ("rabbitmq-corpuslegis"); // Nombre del CONTENEDOR/RECURSO: nombre interno con el que Aspire identifica al contenedor de RabbitMQ en el Dashboard

// Se define el servidor SQL y la base de datos
// TODO: es "legis", no "legit".
var sql_corpuslegit = builder.AddSqlServer("sqlserver") // Nombre del CONTENEDOR/RECURSO: nombre interno con el que Aspire identifica al contenedor de SQL Server en el Dashboard
                            .AddDatabase("corpuslegis-db"); // Nombre de la CONEXIÓN LÓGICA: Aspire inyectará una cadena de conexión en los otros proyectos bajo este nombre

// Se agrega el proyecto de la API
var api_corpuslegis = builder.AddProject<Projects.CorpusLegis_API>("corpuslegis-api")
                            .WithReference(sql_corpuslegit) // se inyecta la cadena de conexión a la base de datos
                            .WithReference(rabbitmq_corpuslegis); // se inyecta la configuración de RabbitMQ

// Se agrega el proyecto del WebApp
builder.AddProject<Projects.CorpusLegis_Web>("corpuslegis-web") // TODO: mejor llamarlo "corpuslegis-web-blazor", por si en el futuro hago una de React.
        .WithExternalHttpEndpoints()
        .WithReference(api_corpuslegis);

// se agrega el proyecto del Worker para la creación de PDFs
builder.AddProject<Projects.CorpusLegis_PdfWorker>("corpuslegis-pdfworker")
    .WithReference(rabbitmq_corpuslegis);

// se agrega el proyecto del Worker para la evaluación de las Rogationes.
builder.AddProject<Projects.CorpusLegis_EscrutinioWorker>("corpuslegis-escrutinioworker")
    .WithReference(api_corpuslegis);

builder.Build().Run();
