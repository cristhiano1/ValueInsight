# ============================================================
# ValueInsight Frontend Migration Script
# Migra el frontend React de Lovable a Visual Studio
# ============================================================
# USO:
#   1. Abre PowerShell como Administrador
#   2. Navega a la carpeta donde descargaste/clonaste este proyecto de Lovable
#   3. Ejecuta: .\scripts\migrate-to-visual-studio.ps1 -TargetPath "C:\ruta\a\tu\solucion\ValueInsight.Frontend"
#
# EJEMPLO:
#   .\scripts\migrate-to-visual-studio.ps1 -TargetPath "C:\Users\TuUsuario\source\repos\ValueInsight\ValueInsight.Frontend"
# ============================================================

param(
    [Parameter(Mandatory = $true)]
    [string]$TargetPath,

    [Parameter(Mandatory = $false)]
    [switch]$BuildForProduction,

    [Parameter(Mandatory = $false)]
    [string]$BackendWwwRoot = ""
)

# --- Colores y helpers ---
function Write-Step($step, $msg) {
    Write-Host "`n[$step] $msg" -ForegroundColor Cyan
}
function Write-Ok($msg) {
    Write-Host "  ✅ $msg" -ForegroundColor Green
}
function Write-Warn($msg) {
    Write-Host "  ⚠️  $msg" -ForegroundColor Yellow
}
function Write-Err($msg) {
    Write-Host "  ❌ $msg" -ForegroundColor Red
}

$ErrorActionPreference = "Stop"
$SourcePath = Split-Path -Parent (Split-Path -Parent $MyInvocation.MyCommand.Path)

Write-Host "============================================================" -ForegroundColor Magenta
Write-Host "  ValueInsight - Frontend Migration Script" -ForegroundColor Magenta
Write-Host "============================================================" -ForegroundColor Magenta
Write-Host ""
Write-Host "  Origen:  $SourcePath"
Write-Host "  Destino: $TargetPath"
Write-Host ""

# ============================================================
# PASO 1: Validar entorno
# ============================================================
Write-Step "1/8" "Validando entorno..."

# Verificar Node.js
try {
    $nodeVersion = node --version
    Write-Ok "Node.js encontrado: $nodeVersion"
} catch {
    Write-Err "Node.js no está instalado. Descárgalo de https://nodejs.org"
    exit 1
}

# Verificar npm
try {
    $npmVersion = npm --version
    Write-Ok "npm encontrado: $npmVersion"
} catch {
    Write-Err "npm no está disponible."
    exit 1
}

# Verificar que el origen existe
if (-not (Test-Path "$SourcePath\package.json")) {
    Write-Err "No se encontró package.json en $SourcePath"
    Write-Err "Ejecuta este script desde la raíz del proyecto Lovable."
    exit 1
}
Write-Ok "Proyecto Lovable encontrado."

# ============================================================
# PASO 2: Crear estructura de destino
# ============================================================
Write-Step "2/8" "Creando estructura de carpetas en destino..."

if (-not (Test-Path $TargetPath)) {
    New-Item -ItemType Directory -Path $TargetPath -Force | Out-Null
    Write-Ok "Carpeta creada: $TargetPath"
} else {
    Write-Warn "La carpeta destino ya existe. Los archivos se sobrescribirán."
}

# Subcarpetas necesarias
$subfolders = @(
    "src",
    "src\components",
    "src\components\ui",
    "src\context",
    "src\data",
    "src\hooks",
    "src\i18n",
    "src\lib",
    "src\pages",
    "src\services",
    "src\test",
    "src\assets",
    "public"
)

foreach ($folder in $subfolders) {
    $fullPath = Join-Path $TargetPath $folder
    if (-not (Test-Path $fullPath)) {
        New-Item -ItemType Directory -Path $fullPath -Force | Out-Null
    }
}
Write-Ok "Estructura de carpetas creada."

# ============================================================
# PASO 3: Copiar archivos de configuración
# ============================================================
Write-Step "3/8" "Copiando archivos de configuración..."

$configFiles = @(
    "package.json",
    "tsconfig.json",
    "tsconfig.app.json",
    "tsconfig.node.json",
    "tailwind.config.ts",
    "postcss.config.js",
    "vite.config.ts",
    "components.json",
    "eslint.config.js",
    "index.html"
)

foreach ($file in $configFiles) {
    $src = Join-Path $SourcePath $file
    $dst = Join-Path $TargetPath $file
    if (Test-Path $src) {
        Copy-Item $src $dst -Force
        Write-Ok "Copiado: $file"
    } else {
        Write-Warn "No encontrado (omitido): $file"
    }
}

# ============================================================
# PASO 4: Copiar código fuente (src/)
# ============================================================
Write-Step "4/8" "Copiando código fuente (src/)..."

$srcSource = Join-Path $SourcePath "src"
$srcTarget = Join-Path $TargetPath "src"

# Copiar todo el contenido de src/ recursivamente
Get-ChildItem -Path $srcSource -Recurse -File | ForEach-Object {
    $relativePath = $_.FullName.Substring($srcSource.Length)
    $destFile = Join-Path $srcTarget $relativePath
    $destDir = Split-Path $destFile -Parent
    
    if (-not (Test-Path $destDir)) {
        New-Item -ItemType Directory -Path $destDir -Force | Out-Null
    }
    
    Copy-Item $_.FullName $destFile -Force
}

$fileCount = (Get-ChildItem -Path $srcSource -Recurse -File).Count
Write-Ok "Copiados $fileCount archivos del código fuente."

# ============================================================
# PASO 5: Copiar archivos públicos (public/)
# ============================================================
Write-Step "5/8" "Copiando archivos públicos (public/)..."

$publicSource = Join-Path $SourcePath "public"
$publicTarget = Join-Path $TargetPath "public"

if (Test-Path $publicSource) {
    Get-ChildItem -Path $publicSource -Recurse -File | ForEach-Object {
        $relativePath = $_.FullName.Substring($publicSource.Length)
        $destFile = Join-Path $publicTarget $relativePath
        $destDir = Split-Path $destFile -Parent
        
        if (-not (Test-Path $destDir)) {
            New-Item -ItemType Directory -Path $destDir -Force | Out-Null
        }
        
        Copy-Item $_.FullName $destFile -Force
    }
    $pubCount = (Get-ChildItem -Path $publicSource -Recurse -File).Count
    Write-Ok "Copiados $pubCount archivos públicos."
} else {
    Write-Warn "No se encontró carpeta public/."
}

# ============================================================
# PASO 6: Instalar dependencias
# ============================================================
Write-Step "6/8" "Instalando dependencias npm..."

Push-Location $TargetPath
try {
    npm install 2>&1 | Out-Null
    Write-Ok "Dependencias instaladas correctamente."
} catch {
    Write-Err "Error instalando dependencias. Ejecuta 'npm install' manualmente en $TargetPath"
}
Pop-Location

# ============================================================
# PASO 7: Verificar que compila
# ============================================================
Write-Step "7/8" "Verificando compilación (build de prueba)..."

Push-Location $TargetPath
try {
    $buildOutput = npm run build 2>&1
    if ($LASTEXITCODE -eq 0) {
        Write-Ok "Build exitoso. El proyecto compila correctamente."
    } else {
        Write-Warn "El build tuvo advertencias. Revisa la salida:"
        Write-Host $buildOutput -ForegroundColor Yellow
    }
} catch {
    Write-Warn "No se pudo ejecutar el build. Revisa los errores manualmente."
}
Pop-Location

# ============================================================
# PASO 8: Copiar a wwwroot (opcional - producción)
# ============================================================
if ($BuildForProduction -and $BackendWwwRoot -ne "") {
    Write-Step "8/8" "Copiando build a wwwroot del backend..."

    $distPath = Join-Path $TargetPath "dist"
    
    if (Test-Path $distPath) {
        # Limpiar wwwroot existente (excepto archivos del backend)
        if (Test-Path $BackendWwwRoot) {
            Get-ChildItem -Path $BackendWwwRoot -Recurse | Remove-Item -Recurse -Force
        } else {
            New-Item -ItemType Directory -Path $BackendWwwRoot -Force | Out-Null
        }

        # Copiar dist/ a wwwroot/
        Copy-Item -Path "$distPath\*" -Destination $BackendWwwRoot -Recurse -Force
        
        $wwwCount = (Get-ChildItem -Path $BackendWwwRoot -Recurse -File).Count
        Write-Ok "Copiados $wwwCount archivos a wwwroot."
        Write-Ok "Ruta: $BackendWwwRoot"
    } else {
        Write-Err "No se encontró la carpeta dist/. ¿El build fue exitoso?"
    }
} else {
    Write-Step "8/8" "Paso de producción omitido (usa -BuildForProduction -BackendWwwRoot para activar)."
}

# ============================================================
# RESUMEN FINAL
# ============================================================
Write-Host ""
Write-Host "============================================================" -ForegroundColor Green
Write-Host "  ✅ MIGRACIÓN COMPLETADA" -ForegroundColor Green
Write-Host "============================================================" -ForegroundColor Green
Write-Host ""
Write-Host "  📁 Frontend copiado a: $TargetPath" -ForegroundColor White
Write-Host ""
Write-Host "  PRÓXIMOS PASOS:" -ForegroundColor Yellow
Write-Host "  1. Abre Visual Studio y agrega la carpeta al proyecto" -ForegroundColor White
Write-Host "  2. En terminal, navega a $TargetPath" -ForegroundColor White
Write-Host "  3. Ejecuta: npm run dev" -ForegroundColor White
Write-Host "  4. Abre http://localhost:8080 en tu navegador" -ForegroundColor White
Write-Host "  5. Inicia tu backend ASP.NET en el puerto 5001" -ForegroundColor White
Write-Host ""
Write-Host "  PARA PRODUCCIÓN:" -ForegroundColor Yellow
Write-Host "  .\scripts\migrate-to-visual-studio.ps1 \" -ForegroundColor Gray
Write-Host "      -TargetPath `"$TargetPath`" \" -ForegroundColor Gray
Write-Host "      -BuildForProduction \" -ForegroundColor Gray
Write-Host "      -BackendWwwRoot `"C:\...\wwwroot`"" -ForegroundColor Gray
Write-Host ""
Write-Host "============================================================" -ForegroundColor Green
