# Linux Container Publishing Steps

## One-Time Setup: Base Image

1. **Download the runtime:**
    - Get it from: <https://repo.steampowered.com/steamrt-images-sniper/snapshots/latest-public-stable/>
    - Currently: `com.valvesoftware.SteamRuntime.Sdk-amd64,i386-sniper-sysroot.tar.gz`
    - Place the file in this folder

2. **Build the base image:**

    ```bash
    docker build -t kriss-journey-env-base:latest -f Dockerfile.steamrtenvironmentsetup .
    ```

    Wait for completion.

## Building the Game

Use the custom `build-docker.ps1` script to build the game in an image based on the base image created in the previous step.
This will use the other Dockerfile.steamrtpublish (which only copies the source files of the game)
