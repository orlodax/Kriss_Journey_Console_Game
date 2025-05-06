#!/bin/bash
# filepath: h:\KrissJourney\krissLauncher.sh
# Universal launcher script for Linux-x64 and macOS-x64

# Get the directory where the script is located
dir="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
log_file="$dir/launch.log"

# Begin logging
echo "=============== NEW LAUNCH ===============" > "$log_file"
echo "Script started at $(date)" >> "$log_file"
echo "Initial working directory: $(pwd)" >> "$log_file"
echo "Script directory: $dir" >> "$log_file"
echo "Script user: $(whoami)" >> "$log_file"
echo "System: $(uname -a)" >> "$log_file"

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

# Make the executable file executable
chmod +x "$dir/Kriss" 2>> "$log_file"

# Set better environment variables for terminal display
export TERM=xterm-256color
export LANG=en_US.UTF-8

# Detect Linux-specific environments
IS_STEAM=false
IS_STEAM_DECK=false
if [[ "$OS_TYPE" == "linux" ]]; then
    if [ -n "$SteamAppId" ] || [ -n "$STEAM_RUNTIME" ] || [[ "$PATH" == *"steam-runtime"* ]]; then
        IS_STEAM=true
        echo "Detected Steam environment" >> "$log_file"
    fi
    
    if [[ "$(whoami)" == "deck" ]] || [ -f "/etc/steamos-release" ]; then
        IS_STEAM_DECK=true
        echo "Detected Steam Deck environment" >> "$log_file"
    fi
fi

# Function to find a terminal by command name
find_terminal() {
    terminal_name=$1
    for path_dir in $(echo "$PATH" | tr ':' ' '); do
        if [ -x "${path_dir}/${terminal_name}" ]; then
            echo "${path_dir}/${terminal_name}"
            return 0
        fi
    done
    
    # Check common locations based on OS
    if [[ "$OS_TYPE" == "linux" ]]; then
        for prefix in "/usr/bin" "/usr/local/bin" "/bin" "/opt/bin"; do
            if [ -x "${prefix}/${terminal_name}" ]; then
                echo "${prefix}/${terminal_name}"
                return 0
            fi
        done
    elif [[ "$OS_TYPE" == "macos" ]]; then
        for prefix in "/usr/bin" "/usr/local/bin" "/bin" "/opt/bin" "/Applications/Utilities"; do
            if [ -x "${prefix}/${terminal_name}" ]; then
                echo "${prefix}/${terminal_name}"
                return 0
            fi
        done
    fi
    
    return 1
}

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
    exec "$dir/Kriss"
    exit 0
fi

# Steam Deck specific handling
if [ "$IS_STEAM_DECK" = true ] && [ "$IS_STEAM" = true ]; then
    echo "Using Steam Deck specific approaches" >> "$log_file"
    
    # Try to use flatpak-spawn to access host
    FLATPAK_SPAWN=$(find_terminal "flatpak-spawn")
    if [ -n "$FLATPAK_SPAWN" ]; then
        KONSOLE_PATH="/usr/bin/konsole"  # Most likely location on Steam Deck
        
        echo "Attempting to escape container with flatpak-spawn" >> "$log_file"
        "$FLATPAK_SPAWN" --host "$KONSOLE_PATH" --separate --workdir "$dir" -e "$dir/Kriss" >> "$log_file" 2>&1
        exit_code=$?
        
        if [ $exit_code -eq 0 ]; then
            echo "Successfully launched via flatpak-spawn" >> "$log_file"
            exit 0
        fi
    fi
fi

# Linux terminal detection
if [[ "$OS_TYPE" == "linux" ]]; then
    # Try available terminals in order of preference
    echo "Searching for available terminals" >> "$log_file"
    
    # 1. Try Konsole
    KONSOLE=$(find_terminal "konsole")
    if [ -n "$KONSOLE" ]; then
        echo "Found Konsole at $KONSOLE" >> "$log_file"
        "$KONSOLE" --separate --workdir "$dir" -e "$dir/Kriss" >> "$log_file" 2>&1
        exit_code=$?
        
        if [ $exit_code -eq 0 ]; then
            echo "Konsole launch succeeded" >> "$log_file"
            exit 0
        fi
    fi
    
    # 2. Try GNOME Terminal
    GNOME_TERMINAL=$(find_terminal "gnome-terminal")
    if [ -n "$GNOME_TERMINAL" ]; then
        echo "Found GNOME Terminal at $GNOME_TERMINAL" >> "$log_file"
        "$GNOME_TERMINAL" --working-directory="$dir" -- "$dir/Kriss" >> "$log_file" 2>&1
        exit_code=$?
        
        if [ $exit_code -eq 0 ]; then
            echo "GNOME Terminal launch succeeded" >> "$log_file"
            exit 0
        fi
    fi
    
    # 3. Try xterm with adaptive font selection
    XTERM=$(find_terminal "xterm")
    if [ -n "$XTERM" ]; then
        echo "Found xterm at $XTERM" >> "$log_file"
        
        # Try to find a suitable font
        FONT=""
        for font in "DejaVu Sans Mono" "Liberation Mono" "Courier New" "Monospace"; do
            if "$XTERM" -font "$font" -e "exit 0" &>/dev/null; then
                FONT="$font"
                break
            fi
        done
        
        # Set font option if found
        if [ -n "$FONT" ]; then
            $XTERM -fa "$FONT" -fs 14 -bg black -fg white -title "Kriss Journey" -e "$dir/Kriss" >> "$log_file" 2>&1
        else
            $XTERM -fs 14 -bg black -fg white -title "Kriss Journey" -e "$dir/Kriss" >> "$log_file" 2>&1
        fi
        
        exit_code=$?
        
        if [ $exit_code -eq 0 ]; then
            echo "xterm launch succeeded" >> "$log_file"
            exit 0
        fi
    fi
fi

# If all terminal attempts fail, run directly
echo "All terminal attempts failed, executing directly" >> "$log_file"
exec "$dir/Kriss"