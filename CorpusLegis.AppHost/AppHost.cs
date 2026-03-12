var builder = DistributedApplication.CreateBuilder(args);

// Message Broker
var rabbitmq_corpuslegis = builder.AddRabbitMQ("rabbitmq-corpuslegis"); // Nombre del CONTENEDOR/RECURSO: nombre interno con el que Aspire identifica al contenedor de RabbitMQ en el Dashboard

// Se define el servidor SQL y la base de datos
var sql_corpuslegis = builder.AddSqlServer("sqlserver-corpuslegis") // Nombre del CONTENEDOR/RECURSO: nombre interno con el que Aspire identifica al contenedor de SQL Server en el Dashboard
                            .AddDatabase("sqlserver-db-corpuslegis"); // Nombre de la CONEXIÓN LÓGICA: Aspire inyectará una cadena de conexión en los otros proyectos bajo este nombre

// Se configura Keycloak para la autentificación y autorización
var keycloakUsername = builder.AddParameter("keycloak-admin-username", value: "admin");
var keycloakPassword = builder.AddParameter("keycloak-admin-password", secret: true, value: "admin");
var keycloak_corpuslegis = builder.AddKeycloak("keycloak-corpuslegis", 8080, keycloakUsername, keycloakPassword)
                                    .WithDataVolume(); // para persistir los datos entre reinicios del contenedor.
                                    //.WithHttpHealthCheck("/health/ready"); // para que Aspire pueda verificar que Keycloak está listo antes de iniciar los otros proyectos que dependen de él.
                                    //.WithHttpHealthCheck(keycloak_corpuslegis.GetEndpoint("http").Property(EndpointProperty.Url) + "/health/ready");
                                    //.WithHttpHealthCheck(keycloak_corpuslegis.GetEndpoint("management").Property(EndpointProperty.Url) + "/health/ready");
var managementUrlExpr = keycloak_corpuslegis.GetEndpoint("management").Property(EndpointProperty.Url);
//keycloak_corpuslegis.WithHttpHealthCheck(ReferenceExpression.Create($"{managementUrlExpr}/health/ready"));
var keycloakHealthCheckUrl = ReferenceExpression.Create($"{managementUrlExpr}/health/ready");
var keycloakAuthority = ReferenceExpression.Create($"{keycloak_corpuslegis.GetEndpoint("http").Property(EndpointProperty.Url)}/realms/CorpusLegis");

// Se agrega el proyecto de la API
var api_corpuslegis = builder.AddProject<Projects.CorpusLegis_API>("api-corpuslegis")
                            .WithReference(sql_corpuslegis) // se inyecta la cadena de conexión a la base de datos
                            .WaitFor(sql_corpuslegis)
                            .WithReference(rabbitmq_corpuslegis) // se inyecta la configuración de RabbitMQ
                            .WithEnvironment("Keycloak__Authority", keycloakAuthority)
                            //.WithEnvironment("ConnectionStrings__keycloak-corpuslegis", keycloak_corpuslegis.GetEndpoint("http").Property(EndpointProperty.Url))
                            //.WithReference(keycloak_corpuslegis) // se inyecta Keycloak.
                            //.WithEnvironment("Keycloak__Url", keycloak_corpuslegis.GetEndpoint("http"))
                            .WaitFor(keycloak_corpuslegis);
                            

// Se agrega el proyecto del WebApp
builder.AddProject<Projects.CorpusLegis_Web>("web-blazor-corpuslegis")
        .WithExternalHttpEndpoints()
        //.WithReference(keycloak_corpuslegis)
        //.WithEnvironment("Keycloak__Url", keycloak_corpuslegis.GetEndpoint("http"))
        .WithEnvironment("Keycloak__Authority", keycloakAuthority)
        //.WithEnvironment("ConnectionStrings__keycloak-corpuslegis", keycloak_corpuslegis.GetEndpoint("http").Property(EndpointProperty.Url))
        .WithReference(api_corpuslegis)
        .WaitFor(keycloak_corpuslegis)
        .WaitFor(api_corpuslegis);

// se agrega el proyecto del Worker para la creación de PDFs
//builder.AddProject<Projects.CorpusLegis_PdfWorker>("pdfworker-corpuslegis")
//    .WithReference(rabbitmq_corpuslegis);

// se agrega el proyecto del Worker para la evaluación de las Rogationes.
//builder.AddProject<Projects.CorpusLegis_EscrutinioWorker>("escrutinioworker-corpuslegis")
//    .WithReference(api_corpuslegis);

builder.Build().Run();
