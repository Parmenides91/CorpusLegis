var builder = DistributedApplication.CreateBuilder(args);

// Redis chaché
var redis_corpuslegis = builder.AddRedis("redis-corpuslegis")
                                .WithRedisInsight(containerName: "redis-insight-corpuslegis");

// Message Broker
var rabbitmq_corpuslegis = builder.AddRabbitMQ("rabbitmq-corpuslegis"); // Nombre del CONTENEDOR/RECURSO: nombre interno con el que Aspire identifica al contenedor de RabbitMQ en el Dashboard

// Se define el servidor SQL y la base de datos
var sql_corpuslegis = builder.AddSqlServer("sqlserver-corpuslegis") // Nombre del CONTENEDOR/RECURSO: nombre interno con el que Aspire identifica al contenedor de SQL Server en el Dashboard
                            .AddDatabase("sqlserver-db-corpuslegis"); // Nombre de la CONEXIÓN LÓGICA: Aspire inyectará una cadena de conexión en los otros proyectos bajo este nombre

// Se define la infraestrcutura de MongoDb.
var mongo_corpuslegis = builder.AddMongoDB("mongo-corpuslegis")
                            .AddDatabase("mongo-db-corpuslegis");

// Se agrega el microservicio de Reputatio.
var reputatio_corpuslegis = builder.AddProject<Projects.CorpusLegis_Reputatio>("reputatio-corpuslegis")
    .WithReference(mongo_corpuslegis)
    .WithReference(rabbitmq_corpuslegis)
    .WaitFor(rabbitmq_corpuslegis)
    ;

// Se configura Keycloak para la autentificación y autorización
var keycloakUsername = builder.AddParameter("keycloak-admin-username", value: "admin");
var keycloakPassword = builder.AddParameter("keycloak-admin-password", secret: true, value: "admin");
var keycloak_corpuslegis = builder.AddKeycloak("keycloak-corpuslegis", 8080, keycloakUsername, keycloakPassword)
                                    .WithDataVolume() // para persistir los datos entre reinicios del contenedor.
                                    ;

var managementUrlExpr = keycloak_corpuslegis.GetEndpoint("management").Property(EndpointProperty.Url);
var keycloakHealthCheckUrl = ReferenceExpression.Create($"{managementUrlExpr}/health/ready");
var keycloakAuthority = ReferenceExpression.Create($"{keycloak_corpuslegis.GetEndpoint("http").Property(EndpointProperty.Url)}/realms/CorpusLegis");

// Se agrega el proyecto de la API
var api_corpuslegis = builder.AddProject<Projects.CorpusLegis_API>("api-corpuslegis")
                            .WithReference(sql_corpuslegis) // se inyecta la cadena de conexión a la base de datos
                            .WaitFor(sql_corpuslegis)
                            .WithReference(rabbitmq_corpuslegis) // se inyecta la configuración de RabbitMQ
                            .WithEnvironment("Keycloak__Authority", keycloakAuthority)
                            .WaitFor(keycloak_corpuslegis)
                            .WithReference(redis_corpuslegis) // se inyecta la configuración de Redis para caché
                            .WithReference(reputatio_corpuslegis)
                            ;
                            

// Se agrega el proyecto del WebApp Blazor.
builder.AddProject<Projects.CorpusLegis_Web>("web-blazor-corpuslegis")
        .WithExternalHttpEndpoints()
        .WithEnvironment("Keycloak__Authority", keycloakAuthority)
        .WithReference(api_corpuslegis)
        .WaitFor(keycloak_corpuslegis)
        .WaitFor(api_corpuslegis);

// se agrega el proyecto del Worker para la creación de PDFs.
var keycloakPdfWorkerClientSecretParam = builder.AddParameter("Keycloak-PdfWorker-ClientSecret", secret: true, value: "s5rqoXEZE7z7EA80ATMpuedxzzmC0H1T");
//var keycloakPdfWorkerClientSecretParam = builder.AddParameter("Keycloak-PdfWorker-ClientSecret", secret: true); // de esta manera no arrancará hasta que no le pongas el Secret desde el panel de Aspire.
builder.AddProject<Projects.CorpusLegis_PdfWorker>("pdfworker-corpuslegis")
    .WithReference(rabbitmq_corpuslegis)
    .WithEnvironment("Keycloak__Authority", keycloakAuthority)
    .WithEnvironment("Keycloak-PdfWorker-ClientSecret", keycloakPdfWorkerClientSecretParam)
    .WaitFor(keycloak_corpuslegis)
    .WaitFor(api_corpuslegis);

// se agrega el proyecto del Worker para la evaluación de las Rogationes.
var keycloakEscrutinioWorkerClientSecretParam = builder.AddParameter("Keycloak-EscrutinioWorker-ClientSecret", secret: true, value: "tN7FxsI1QloNMnIrdkQAzs6RAhIazXIO");
//var keycloakEscrutinioWorkerClientSecretParam = builder.AddParameter("Keycloak-EscrutinioWorker-ClientSecret", secret: true); // de esta manera no arrancará hasta que no le pongas el Secret desde el panel de Aspire.
builder.AddProject<Projects.CorpusLegis_EscrutinioWorker>("escrutinioworker-corpuslegis")
    .WithReference(api_corpuslegis)
    .WithEnvironment("Keycloak__Authority", keycloakAuthority)
    .WithEnvironment("Keycloak__EscrutinioWorker__ClientSecret", keycloakEscrutinioWorkerClientSecretParam)
    .WaitFor(keycloak_corpuslegis)
    .WaitFor(api_corpuslegis);



builder.Build().Run();
