param(
    [string]$Src,
    [string]$Out,
    [int]$W = 600,
    [int]$H = 844,
    [int]$BottomMaxAlpha = 190,
    [int]$TopMaxAlpha = 64,
    [double]$BottomHeight = 0.32,
    [double]$BottomCurve = 2.0
)

Add-Type -AssemblyName System.Drawing

$dir = Split-Path $Out -Parent
New-Item -ItemType Directory -Force -Path $dir | Out-Null

$srcImg = [System.Drawing.Image]::FromFile($Src)
$bmp = New-Object System.Drawing.Bitmap($W, $H, ([System.Drawing.Imaging.PixelFormat]::Format32bppArgb))
$g = [System.Drawing.Graphics]::FromImage($bmp)
$g.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
$g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::HighQuality

# cover scale: enlarge to fill target, crop overflow
$scale = [Math]::Max($W / [double]$srcImg.Width, $H / [double]$srcImg.Height)
$sw = [int]($srcImg.Width * $scale)
$sh = [int]($srcImg.Height * $scale)
$dx = [int](($W - $sw) / 2)
$dy = [int](($H - $sh) / 2)
$g.DrawImage($srcImg, $dx, $dy, $sw, $sh)

# bottom gradient (text readability): transparent -> BottomMaxAlpha black over bottom BottomHeight
# curve: alpha = MaxAlpha * t^BottomCurve (default 2.0 = quadratic, closer to 1 = darker sooner)
$gradHeight = [int]($H * $BottomHeight)
for ($i = 0; $i -lt $gradHeight; $i++) {
    $t = $i / $gradHeight
    $alpha = [int]($BottomMaxAlpha * [Math]::Pow($t, $BottomCurve))
    $y = $H - $gradHeight + $i
    $brush = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb($alpha, 0, 0, 0))
    $g.FillRectangle($brush, 0, $y, $W, 1)
    $brush.Dispose()
}

# top subtle darkening (cost/title readability): top 18% up to TopMaxAlpha black
$topHeight = [int]($H * 0.18)
for ($i = 0; $i -lt $topHeight; $i++) {
    $t = 1 - ($i / $topHeight)
    $alpha = [int]($TopMaxAlpha * $t * $t)
    $brush = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb($alpha, 0, 0, 0))
    $g.FillRectangle($brush, 0, $i, $W, 1)
    $brush.Dispose()
}

$g.Dispose()
$bmp.Save($Out, [System.Drawing.Imaging.ImageFormat]::Png)
$bmp.Dispose()
$srcImg.Dispose()
Write-Output "saved: $Out"
