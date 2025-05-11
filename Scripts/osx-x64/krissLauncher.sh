#!/bin/bash
# Script to launch the Kriss console application in a new Terminal window for Steam on macOS.

# Get the directory where the script is located
dir="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
log_file="$dir/launch.log"

# Function to log messages
log() {
  echo "$(date '+%Y-%m-%d %H:%M:%S') - $1" >> "$log_file"
}

# Log script start
log "Launcher script started."

# Create a lock file with a unique name
LOCK_FILE="/tmp/kriss_lock_$$.tmp"
touch "$LOCK_FILE"
log "Created lock file: $LOCK_FILE"

# Check if Kriss executable exists
if [ ! -f "$dir/Kriss" ]; then
  log "ERROR: Kriss executable not found at $dir/Kriss"
  rm -f "$LOCK_FILE"
  exit 1
else
  log "Kriss executable found at $dir/Kriss"
fi

# Check if Kriss executable is executable
if [ ! -x "$dir/Kriss" ]; then
  log "ERROR: Kriss executable is not executable. Attempting to change permissions."
  chmod +x "$dir/Kriss"
  if [ $? -ne 0 ]; then
    log "ERROR: Failed to change permissions on Kriss executable."
    rm -f "$LOCK_FILE"
    exit 1
  else
    log "Successfully changed permissions on Kriss executable."
  fi
else
  log "Kriss executable is executable."
fi

# Create a temporary script to launch Kriss and clean up the lock file
TEMP_SCRIPT="/tmp/kriss_temp_launcher_$$.sh"
cat > "$TEMP_SCRIPT" <<EOS
#!/bin/bash
cd "$dir"
trap 'rm -f "$LOCK_FILE"' EXIT INT TERM
"$dir/Kriss"
EOS
chmod +x "$TEMP_SCRIPT"
log "Created temporary launcher script at: $TEMP_SCRIPT"

# Launch Terminal with the temporary script
log "Launching Kriss in a new Terminal window and waiting for it to close."
osascript <<EOF
tell application "Terminal"
  do script "\"$TEMP_SCRIPT\""
end tell
EOF
LAUNCH_RESULT=$?
log "osascript completed with result code: $LAUNCH_RESULT"

if [ "$LAUNCH_RESULT" -ne 0 ]; then
  log "ERROR: Failed to launch Kriss via osascript."
  rm -f "$LOCK_FILE" "$TEMP_SCRIPT"
  exit 1
else
  log "Kriss launched successfully. Waiting for it to exit."
fi

# Wait for the lock file to be removed (i.e., Terminal/game has exited)
while [ -e "$LOCK_FILE" ]; do
  sleep 1
done

log "Terminal window closed, Kriss has exited."

# Clean up the temporary script
rm -f "$TEMP_SCRIPT"

# Log script end
log "Launcher script finished."
exit 0