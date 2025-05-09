#!/bin/bash
# Universal launcher for Kriss Journey
# This script handles Linux/macOS execution with enhanced Steam compatibility
# Version: 3.0 - Improved bundled terminal emulator for consistent display across all Linux environments

# Debug mode can be activated by passing --debug as the first argument
DEBUG_MODE=false
FORCE_XTERM_CONFIG=false

if [ "$1" == "--debug" ]; then
    DEBUG_MODE=true
    shift
fi

if [ "$1" == "--force-xterm-config" ]; then
    FORCE_XTERM_CONFIG=true
    shift
fi

# Get the directory where the script is located
dir="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
log_file="$dir/launch.log"
error_log="$dir/error.log"
debug_log="$dir/debug.log"

# Begin logging
echo "=============== NEW LAUNCH ===============" > "$log_file"
echo "Script started at $(date)" >> "$log_file"
echo "Working directory: $(pwd)" >> "$log_file"
echo "Script directory: $dir" >> "$log_file"
echo "Script user: $(whoami)" >> "$log_file"
echo "System info: $(uname -a)" >> "$log_file"
if [ "$DEBUG_MODE" = true ]; then
    echo "DEBUG MODE ENABLED" >> "$log_file"
    echo "=============== DEBUG LOG ===============" > "$debug_log"
    echo "Debug session started at $(date)" >> "$debug_log"
fi

# Function to handle errors with detailed diagnostics
log_error() {
    echo "ERROR: $1" | tee -a "$log_file" "$error_log"
}

# Function to log debug information
log_debug() {
    if [ "$DEBUG_MODE" = true ]; then
        echo "DEBUG: $1" | tee -a "$debug_log"
    fi
}

# Function to check if a binary is available and executable
check_executable() {
    local binary_path="$1"
    if [ -f "$binary_path" ] && [ -x "$binary_path" ]; then
        return 0
    else
        return 1
    fi
}

# Function to launch the game using bundled kitty terminal or fallback to system terminal
launch_with_terminal() {
    local environment="$1"  # "steam", "steam-deck", or "linux"
    
    echo "Launching game in $environment environment" >> "$log_file"
    
    # First check for bundled kitty terminal
    local kitty_path="$dir/terminal/bin/kitty"
    if check_executable "$kitty_path"; then
        echo "Found bundled kitty terminal, using it" >> "$log_file"
        
        # Set kitty configuration
        export KITTY_CONFIG_DIRECTORY="$dir/terminal/share/kitty"
        
        # Launch with kitty
        echo "Executing game via bundled kitty terminal..." >> "$log_file"
        
        # Steam Deck specific settings for kitty
        if [ "$environment" == "steam-deck" ]; then
            log_debug "Setting Steam Deck specific kitty parameters"
            export KITTY_DISABLE_WAYLAND=1
        fi
        
        # Execute the game with the bundled terminal
        # Set minimum window dimensions and ensure window controls work
        "$kitty_path" --title "Kriss Journey" --start-as normal -o remember_window_size=no -o initial_window_width=800 -o initial_window_height=600 -o confirm_os_window_close=0 "$dir/Kriss" >> "$log_file" 2>&1 &
        
        # Store PID for waiting
        local terminal_pid=$!
        
        # Wait for kitty to finish
        wait $terminal_pid
        local exit_code=$?
        
        echo "Game execution via bundled terminal completed with exit code $exit_code" >> "$log_file"
        return $exit_code
    fi
    
    # Fall back to system kitty if available
    if command -v kitty &>/dev/null; then
        echo "Bundled terminal not found, but system kitty is available" >> "$log_file"
        
        # Launch with system kitty with proper window size and controls
        kitty --title "Kriss Journey" -o remember_window_size=no -o initial_window_width=800 -o initial_window_height=600 -o confirm_os_window_close=0 "$dir/Kriss" >> "$log_file" 2>&1 &
        
        # Wait for kitty to finish
        wait
        local exit_code=$?
        
        echo "Game execution via system kitty completed with exit code $exit_code" >> "$log_file"
        return $exit_code
    fi
    
    # Fall back to system xterm if bundled terminal isn't available
    echo "Kitty terminal not found, checking for system xterm" >> "$log_file"
    
    # Check if xterm is available
    if ! command -v xterm &>/dev/null; then
        echo "ERROR: No suitable terminal emulator found in system" >> "$log_file"
        log_error "Terminal emulator not found in $environment environment - cannot launch game"
        return 1
    fi
    
    echo "Found xterm, using it with enhanced color config" >> "$log_file"
    
    # Create a temporary Xresources file for color configuration
    XRESOURCES_TMP="$dir/xterm-colors.tmp"
    cat > "$XRESOURCES_TMP" << EOF
! Custom color settings for Kriss Journey on xterm
xterm*background: black
xterm*foreground: white
xterm*cursorColor: green
xterm*faceName: Monospace
xterm*faceSize: 14
xterm*saveLines: 1000
xterm*scrollBar: true
xterm*rightScrollBar: true
xterm*title: Kriss Journey
! Standard colors
xterm*color0: rgb:00/00/00
xterm*color1: rgb:ff/40/40
xterm*color2: rgb:40/ff/40
xterm*color3: rgb:ff/ff/40
xterm*color4: rgb:40/40/ff
xterm*color5: rgb:ff/40/ff
xterm*color6: rgb:40/ff/ff
xterm*color7: rgb:ff/ff/ff
xterm*color8: rgb:60/60/60
xterm*color9: rgb:ff/80/80
xterm*color10: rgb:80/ff/80
xterm*color11: rgb:ff/ff/80
xterm*color12: rgb:80/80/ff
xterm*color13: rgb:ff/80/ff
xterm*color14: rgb:80/ff/ff
xterm*color15: rgb:ff/ff/ff
EOF
    
    # Load xterm resources
    xrdb -merge "$XRESOURCES_TMP" >> "$log_file" 2>&1
    
    # Launch xterm with our custom settings
    xterm -bg black -fg white -fa "Monospace" -fs 14 -title "Kriss Journey" -e "$dir/Kriss" >> "$log_file" 2>&1 &
    
    # Store the PID for checking and waiting
    local xterm_pid=$!
    
    # Wait a bit to see if xterm started in Steam environment
    if [[ "$environment" == "steam"* ]]; then
        sleep 1
        # Check if process is running
        if kill -0 $xterm_pid 2>/dev/null; then
            echo "xterm launch successful (PID: $xterm_pid)" >> "$log_file"
            wait $xterm_pid # Wait for xterm to finish
            local exit_code=$?
            echo "Game execution via xterm completed with exit code $exit_code" >> "$log_file"
        else
            echo "ERROR: xterm launch failed" >> "$log_file"
            log_error "xterm launch failed in $environment environment - cannot launch game"
            rm -f "$XRESOURCES_TMP" # Clean up temp file
            return 1
        fi
    else
        # For non-Steam environment, just wait for xterm to finish
        wait $xterm_pid
        local exit_code=$?
        echo "Game execution via xterm completed with exit code $exit_code" >> "$log_file"
    fi
    
    # Clean up
    rm -f "$XRESOURCES_TMP"
    return $exit_code
}

# Function to directly launch the game executable without a terminal
direct_launch() {
    echo "Attempting direct launch of game executable..." >> "$log_file"
    "$dir/Kriss" >> "$log_file" 2>&1
    local exit_code=$?
    echo "Direct execution completed with exit code $exit_code" >> "$log_file"
    return $exit_code
}
# Function to directly launch the game executable without a terminal
direct_launch() {
    echo "Attempting direct launch of game executable..." >> "$log_file"
    "$dir/Kriss" >> "$log_file" 2>&1
    local exit_code=$?
    echo "Direct execution completed with exit code $exit_code" >> "$log_file"
    return $exit_code
}

# Detect OS
OS_TYPE="unknown"
if [[ "$(uname)" == "Darwin" ]]; then
    OS_TYPE="macos"
    echo "Detected macOS environment" >> "$log_file"
elif [[ "$(uname)" == "Linux" ]]; then
    OS_TYPE="linux"
    echo "Detected Linux environment" >> "$log_file"
else
    echo "Warning: Unsupported OS detected. Attempting to launch anyway." >> "$log_file"
fi

# Detect Steam Deck specifically (only for additional configuration)
IS_STEAM_DECK=false
if [[ "$OS_TYPE" == "linux" ]]; then
    # Steam Deck detection remains useful for specific settings
    if [[ "$(uname -n)" == *"steamdeck"* ]] || [[ "$(whoami)" == "deck" ]] || [ -f "/etc/steamos-release" ] || [ -n "$SteamDeck" ]; then
        IS_STEAM_DECK=true
        echo "Detected Steam Deck environment" >> "$log_file"
    fi
    
    # Log Steam environment variables for debugging if present
    if [ -n "$SteamAppId" ] || [ -n "$STEAM_RUNTIME" ] || [[ "$PATH" == *"steam-runtime"* ]]; then
        echo "Detected explicit Steam environment variables" >> "$log_file"
        env | grep -i steam >> "$log_file"
    fi
fi

# Log file permissions
echo "File permissions:" >> "$log_file"
ls -la "$dir/Kriss" >> "$log_file" 2>&1
file "$dir/Kriss" >> "$log_file" 2>&1

# Make the executable file executable
chmod +x "$dir/Kriss" 2>> "$log_file"
if [ -f "$dir/terminal/bin/kitty" ]; then
    chmod +x "$dir/terminal/bin/kitty" 2>> "$log_file"
    echo "Made bundled terminal executable" >> "$log_file"
fi

# Set better environment variables for terminal display
export TERM=xterm-256color
export COLORTERM=truecolor
export LANG=en_US.UTF-8

# Debug: Detect terminal capabilities
if [ "$DEBUG_MODE" = true ]; then
    log_debug "Terminal capabilities:"
    log_debug "TERM=$TERM"
    log_debug "COLORTERM=$COLORTERM"
    
    # Check color support
    if command -v tput &>/dev/null; then
        log_debug "Colors supported by current terminal: $(tput colors 2>/dev/null || echo 'unknown')"
    fi
    
    # Check if terminal emulator can be detected
    if [ -n "$TERM_PROGRAM" ]; then
        log_debug "Terminal program: $TERM_PROGRAM"
    else
        log_debug "Terminal program not detected via TERM_PROGRAM"
    fi
    
    # Check for common terminal emulators
    for term in kitty gnome-terminal konsole xterm xfce4-terminal terminator alacritty rxvt-unicode yakuake tilix urxvt terminology; do
        if command -v $term &>/dev/null; then
            log_debug "Found terminal emulator: $term ($(which $term))"
        fi
    done
    
    # Check if bundled terminal exists
    if [ -f "$dir/terminal/bin/kitty" ]; then
        log_debug "Bundled kitty terminal found at: $dir/terminal/bin/kitty"
        log_debug "Kitty version: $($dir/terminal/bin/kitty --version 2>/dev/null || echo 'unknown')"
    else
        log_debug "Bundled kitty terminal NOT found"
    fi
fi

# macOS specific handling
if [[ "$OS_TYPE" == "macos" ]]; then
    echo "Using macOS specific approaches" >> "$log_file"
    
    # On macOS, use Terminal.app or iTerm if available
    if [ -d "/Applications/iTerm.app" ]; then
        echo "Found iTerm2, launching with it" >> "$log_file"
        osascript << EOF >> "$log_file" 2>&1
tell application "iTerm"
    activate
    set newWindow to (create window with default profile)
    tell current session of newWindow
        write text "cd \"$dir\" && ./Kriss"
    end tell
end tell
EOF
        exit_code=$?
        
        if [ $exit_code -eq 0 ]; then
            echo "iTerm2 launch succeeded" >> "$log_file"
            exit 0
        fi
    elif [ -d "/Applications/Utilities/Terminal.app" ]; then
        echo "Found Terminal.app, launching with it" >> "$log_file"
        osascript << EOF >> "$log_file" 2>&1
tell application "Terminal"
    activate
    do script "cd \"$dir\" && ./Kriss"
end tell
EOF
        exit_code=$?
        
        if [ $exit_code -eq 0 ]; then
            echo "Terminal.app launch succeeded" >> "$log_file"
            exit 0
        fi
    fi
    
    # If AppleScript approaches fail, try a direct launch
    echo "macOS direct execution" >> "$log_file"
    direct_launch
    exit $?
fi

# Special handling for Steam environment (including Steam Deck and all Linux environments)
if [[ "$OS_TYPE" == "linux" ]]; then
    # For all Linux environments, set Steam variables
    IS_STEAM=true
    echo "Using Linux launch setup (Steam runtime compatible)" >> "$log_file"
    
    # Configure terminal capabilities
    if command -v stty &>/dev/null; then
        stty sane 2>/dev/null || true
        echo "Terminal configured with stty" >> "$log_file"
    fi

    # Initialize terminal capabilities using tput
    if command -v tput &>/dev/null; then
        tput init 2>/dev/null || true
        tput clear 2>/dev/null || true
        tput sgr0 2>/dev/null || true  # Reset attributes
        echo "Terminal initialized with tput" >> "$log_file"
    fi

    # Try to configure proper Unicode and UTF-8 support
    export LC_ALL=en_US.UTF-8 2>/dev/null || export LC_ALL=C.UTF-8 2>/dev/null || true
    export LC_CTYPE=en_US.UTF-8 2>/dev/null || export LC_CTYPE=C.UTF-8 2>/dev/null || true

    # Run ncurses initialization if available
    if command -v infocmp &>/dev/null; then
        echo "Running ncurses initialization" >> "$log_file"
        terminfo=$(infocmp -x 2>/dev/null)
        if [ $? -eq 0 ]; then
            echo "Successfully retrieved terminal info" >> "$log_file"
        else
            echo "Could not retrieve terminal info" >> "$log_file"
        fi
    fi

    # Report actual terminal settings
    echo "TERM=$TERM" >> "$log_file"
    echo "COLORTERM=$COLORTERM" >> "$log_file"
    echo "LANG=$LANG" >> "$log_file"
    echo "LC_ALL=$LC_ALL" >> "$log_file"
    echo "LC_CTYPE=$LC_CTYPE" >> "$log_file"

    # Check for required shared libraries and report if any are missing
    required_libs=(
        "libicu"
        "libssl"
        "libcrypto"
        "libz"
        "libstdc++"
    )

    echo "Checking for required libraries:" >> "$log_file"
    for lib in "${required_libs[@]}"; do
        if ! ldconfig -p 2>/dev/null | grep -q "$lib"; then
            echo "WARNING: $lib appears to be missing from the system" >> "$log_file"
        else
            echo "Found $lib in system libraries" >> "$log_file"
        fi
    done

    # Set LD_LIBRARY_PATH to include game directory
    echo "Setting LD_LIBRARY_PATH to include game directory" >> "$log_file"
    export LD_LIBRARY_PATH="$dir:$LD_LIBRARY_PATH"

    # Ensure .NET specific environment variables are properly set
    export DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1
    echo "DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1" >> "$log_file"

    # Critical fix for console app on Steam 
    export TERM_PROGRAM=steamdeck-console
    export DOTNET_EnableWriteXorExecute=0
    export DOTNET_EnableTelemetry=0
    
    # Debug environment variable for color handling
    export KRISS_DEBUG_COLORS=$DEBUG_MODE
    export KRISS_USE_HIGH_CONTRAST=true
    export KRISS_FORCE_COLOR_SCHEME=true
    
    # Set Steam Deck specific variables
    if [ "$IS_STEAM_DECK" = true ]; then
        echo "Setting Steam Deck specific variables" >> "$log_file"
        export SDL_VIDEODRIVER=x11
        echo "Set SDL_VIDEODRIVER=x11 for Steam Deck" >> "$log_file"
        
        # Launch game with terminal on Steam Deck
        launch_with_terminal "steam-deck"
        exit_code=$?
    else
        # For regular Linux
        echo "Launching on regular Linux environment" >> "$log_file"
        launch_with_terminal "linux"
        exit_code=$?
    fi
    
    # If terminal launch failed, try direct launch as last resort
    if [ $exit_code -ne 0 ]; then
        echo "Terminal launch failed, attempting direct launch as fallback" >> "$log_file"
        direct_launch
        exit_code=$?
    fi
    
    exit $exit_code
else
    echo "Non-Linux/non-macOS environment detected, no appropriate launcher available" >> "$log_file"
    log_error "No appropriate launcher for this environment"
    exit 1
fi