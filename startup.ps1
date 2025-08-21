param (
    [switch] $debug = $false,
    [switch] $useReactServer = $false
)
$ErrorActionPreference = "Stop"


if ($useReactServer -eq $true) {
    docker compose -f docker-compose.yml -f docker-compose.reactserver.yml -f docker-compose.coreapi.yml up -d --build
    return
}
if ($debug -eq $true) {
    docker compose up -d --build
}
else {
    docker compose -f docker-compose.yml -f docker-compose.coreapi.yml up -d --build
}
npm run dev --prefix src/phaser-client