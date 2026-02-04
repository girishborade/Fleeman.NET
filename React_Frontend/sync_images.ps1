# Define source generic images (assuming they are already in the images folder)
$imgDir = "C:\Users\GIRI\Desktop\CDAC Project\Fleeman.NET\React_Frontend\public\images"

# Map specific cars to their categories
$mappings = @{
    # Hatchbacks
    "swift.jpg" = "hatchback.jpg"
    "i20.jpg" = "hatchback.jpg"
    "altroz.jpg" = "hatchback.jpg"
    "kwid.jpg" = "hatchback.jpg"
    "wagonr.jpg" = "hatchback.jpg"
    "grand_i10.jpg" = "hatchback.jpg"
    "tiago.jpg" = "hatchback.jpg"
    "celerio.jpg" = "hatchback.jpg"
    "ignis.jpg" = "hatchback.jpg"
    "baleno.jpg" = "hatchback.jpg"

    # Sedans
    "swift_dzire.jpg" = "sedan.jpg"
    "honda_city.jpg" = "sedan.jpg"
    "verna.jpg" = "sedan.jpg"
    "amaze.jpg" = "sedan.jpg"
    "slavia.jpg" = "sedan.jpg"
    "virtus.jpg" = "sedan.jpg"
    "yaris.jpg" = "sedan.jpg"
    "civic.jpg" = "sedan.jpg"
    "rapid.jpg" = "sedan.jpg"
    "sunny.jpg" = "sedan.jpg"
    "mercedes_c_class.jpg" = "luxury.jpg" # Luxury Sedan
    "bmw3.jpg" = "luxury.jpg"
    "a6.jpg" = "luxury.jpg"
    "bmw5.jpg" = "luxury.jpg"
    "xf.jpg" = "luxury.jpg"
    "lexus_es.jpg" = "luxury.jpg"

    # SUVs
    "creta.jpg" = "suv.jpg"
    "hyundai_creta.jpg" = "suv.jpg"
    "seltos.jpg" = "suv.jpg"
    "harrier.jpg" = "suv.jpg"
    "xuv700.jpg" = "suv.jpg"
    "hector.jpg" = "suv.jpg"
    "toyota_fortuner.jpg" = "suv.jpg"
    "hyryder.jpg" = "suv.jpg"
    "elevate.jpg" = "suv.jpg"
    "q5.jpg" = "luxury.jpg" # Luxury SUV
    "glc.jpg" = "luxury.jpg"
    "xc60.jpg" = "luxury.jpg"

    # MUVs
    "innova_crysta.jpg" = "muv.jpg"
    "innova.jpg" = "muv.jpg"
    "ertiga.jpg" = "muv.jpg"
    "carens.jpg" = "muv.jpg"
    "marazzo.jpg" = "muv.jpg"
    "rumion.jpg" = "muv.jpg"

    # Compact SUVs
    "tata_nexon.jpg" = "compact_suv.jpg"
    "venue.jpg" = "compact_suv.jpg"
    "sonet.jpg" = "compact_suv.jpg"
    "punch.jpg" = "compact_suv.jpg"
    "magnite.jpg" = "compact_suv.jpg"
    "kiger.jpg" = "compact_suv.jpg"

    # Electric
    "nexon_ev.jpg" = "electric.jpg"
    "zs_ev.jpg" = "electric.jpg"
    "kona_ev.jpg" = "electric.jpg"
    "tiago_ev.jpg" = "electric.jpg"
    "xuv400_ev.jpg" = "electric.jpg"

    # Pickups
    "dmax.jpg" = "pickup.jpg"
    "hilux.jpg" = "pickup.jpg"
    "bolero_pickup.jpg" = "pickup.jpg"
    "yodha.jpg" = "pickup.jpg"
    "vcross.jpg" = "pickup.jpg"
}

Write-Host "Syncing car images..." -ForegroundColor Cyan

foreach ($target in $mappings.Keys) {
    $source = $mappings[$target]
    $sourcePath = Join-Path $imgDir $source
    $targetPath = Join-Path $imgDir $target

    if (Test-Path $sourcePath) {
        Copy-Item -Path $sourcePath -Destination $targetPath -Force
        Write-Host "Created $target (from $source)" -ForegroundColor Green
    } else {
        Write-Host "Source image $source not found!" -ForegroundColor Red
    }
}

Write-Host "Image sync complete!" -ForegroundColor Cyan
