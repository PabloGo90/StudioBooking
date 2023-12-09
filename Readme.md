# Primera ejecucion: 
1. ubicarse en directorio
2. `code -r ../DIYSandwich`
3. `dotnet tool install --global dotnet-ef`
4. `dotnet ef migrations add InitialCreate`
5. `dotnet ef database update`

# Tras actualizar modelo:
1. Cuando se quiere aplicar un cambio `dotnet ef migrations add descripciondecambio`
2. actualiza base de datos `dotnet ef database update`

# Para borrar
1. Eliminar carpeta migration
2. ejecutar `dotnet ef database drop -f -v`
3. crear denuevo

# Docker
1. `docker build -t pablog0/tattoostudiobooking:version1.1 .`
2. `docker volume create booking-db`
3. `docker run -p 80:80 --mount type=volume,src=booking-db,target=/etc/studiobooking pablog0/tattoostudiobooking:version1.1 .`
4. `docker push pablog0/tattoostudiobooking:version1.1`

# Otros
- como hacer archivo md [comandos]( https://docs.github.com/en/get-started/writing-on-github/getting-started-with-writing-and-formatting-on-github/basic-writing-and-formatting-syntax)
- hacer [webApi](https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-web-api?view=aspnetcore-8.0&tabs=visual-studio-code)
- hacer [EF core sqllite](https://learn.microsoft.com/en-us/ef/core/get-started/overview/first-app?tabs=netcore-cli)


