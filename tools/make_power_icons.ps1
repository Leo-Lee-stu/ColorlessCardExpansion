param(
    [string]$Src,
    [string]$Name,
    [string]$OutDir
)

Add-Type -AssemblyName System.Drawing

# Crop a square region from the center-ish of the card art, then scale to icon sizes.
$srcImg = [System.Drawing.Image]::FromFile($Src)
$crop = [Math]::Min($srcImg.Width, $srcImg.Height)
$sx = [int](($srcImg.Width - $crop) / 2)
$sy = [int](($srcImg.Height - $crop) * 0.28)

foreach ($size in @(128, 64)) {
    $bmp = New-Object System.Drawing.Bitmap($size, $size, ([System.Drawing.Imaging.PixelFormat]::Format32bppArgb))
    $g = [System.Drawing.Graphics]::FromImage($bmp)
    $g.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
    $g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::HighQuality
    $g.DrawImage($srcImg, (New-Object System.Drawing.Rectangle(0, 0, $size, $size)), (New-Object System.Drawing.Rectangle($sx, $sy, $crop, $crop)), [System.Drawing.GraphicsUnit]::Pixel)
    $sub = if ($size -eq 128) { '' } else { 'Small\' }
    $dir = Join-Path $OutDir "powers\$sub"
    New-Item -ItemType Directory -Force -Path $dir | Out-Null
    $path = Join-Path $dir "$Name.png"
    $bmp.Save($path, [System.Drawing.Imaging.ImageFormat]::Png)
    $bmp.Dispose(); $g.Dispose()
    Write-Output "saved: $path"
}
$srcImg.Dispose()
