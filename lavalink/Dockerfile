FROM fredboat/lavalink:master

WORKDIR /opt/Lavalink

EXPOSE 2333

# Copy the config file
COPY application.yaml ./

ENTRYPOINT ["java", "-Djdk.tls.client.protocols=TLSv1.1,TLSv1.2", "-Xmx4G", "-jar", "Lavalink.jar"]