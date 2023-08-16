FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src

# Copy over the project file from our repo (relative to this Dockerfile) into a directory in the build container.
COPY ["ShoppingCart.csproj", "ShoppingCart/"]

# Pull the project's dependencies.
RUN dotnet restore "ShoppingCart/ShoppingCart.csproj"

# Move the rest of the project files over. This can technically be done first (ie. replaces the first COPY command).
COPY . .

# Build the project to the /app/build directory. This build output stays in the build container.
WORKDIR "/src/ShoppingCart"
RUN dotnet build "ShoppingCart.csproj" -c Release -o /app/build

# Create a new "publish" container and compile the application into the /app/publish directory.
FROM build AS publish
RUN dotnet publish "ShoppingCart.csproj" -c Release -o /app/publish

# Create the final output container that comes with the ASP.NET runtime.
FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS final
WORKDIR /app
EXPOSE 80

# Move just the publishing artifacts to this output container.
COPY --from=publish /app/publish .

# Run the compiled ShoppingCart.dll.
ENTRYPOINT ["dotnet", "ShoppingCart.dll"]
