param (
    [switch] $debug = $false
)
$ErrorActionPreference = "Stop"


if ($debug -eq $true) {
    docker compose up -d --build
}
else {
    docker compose -f docker-compose.yml -f docker-compose.coreapi.yml up -d --build
}


npm run dev --prefix src/phaser-client