var builder = DistributedApplication.CreateBuilder(args);

// Elasticsearch container tanimi
var elasticsearch = builder.AddContainer(
        "starter-elasticsearch",                           // Container adi (Aspire icinde referans ve DNS ismi)
        "docker.elastic.co/elasticsearch/elasticsearch",   // Kullanilacak Elasticsearch Docker image
        "8.11.1")                                          // Image versiyonu
                                                           // Elasticsearch'u single-node modda calistiriyoruz.
                                                           // Local development ortaminda cluster kurmaya gerek yok.
    .WithEnvironment("discovery.type", "single-node")

    // Guvenlik ozelliklerini kapatiyoruz.
    // Local ortamda sertifika ve authentication karmasasini onler.
    .WithEnvironment("xpack.security.enabled", "false")

    // JVM heap boyutunu sinirliyoruz.
    // Gelistirme ortaminda gereksiz bellek tuketimini engeller.
    .WithEnvironment("ES_JAVA_OPTS", "-Xms512m -Xmx512m")

    // Elasticsearch'u sabit bir porttan dis dunyaya aciyoruz.
    // Bu sayede her calistirmada ayni port uzerinden erisilebilir olur.
    .WithHttpEndpoint(port: 9200, targetPort: 9200, name: "http")

    // Elasticsearch verilerinin kalici olmasi icin volume tanimliyoruz.
    // Container silinse bile index verileri kaybolmaz.
    .WithVolume("starter-es-data", "/usr/share/elasticsearch/data")

    // Container lifecycle'ini persistent olarak ayarliyoruz.
    // Aspire durup tekrar calistiginda container yeniden kullanilabilir.
    .WithLifetime(ContainerLifetime.Persistent);

// Kibana container tanimi
var kibana = builder.AddContainer(
        "starter-kibana",                                  // Kibana container adi
        "docker.elastic.co/kibana/kibana",                 // Kibana Docker image
        "8.11.1")                                          // Image versiyonu
    // Kibana'nin Elasticsearch'e nasil baglanacagini belirtiyoruz.
    // IP veya localhost yerine container adini kullaniyoruz.
    // Aspire otomatik olarak internal Docker network ve DNS saglar.
    .WithEnvironment("ELASTICSEARCH_HOSTS", "http://starter-elasticsearch:9200")

    // Kibana'yi dis dunyaya actigimiz HTTP portu
    .WithHttpEndpoint(port: 5601, targetPort: 5601, name: "http")

    // Kibana container'i da persistent olarak calissin.
    // Aspire durup tekrar calistiginda container yeniden kullanilabilir.
    .WithLifetime(ContainerLifetime.Persistent);


// Tanimlanan tum container'lari ayaða kaldiriyoruz
builder.Build().Run();
