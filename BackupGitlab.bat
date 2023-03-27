
@echo off
set BACKUP_DIR=D:\GitLabBackup
set GITLAB_URL=https://gitlab.com/
set GITLAB_TOKEN=gitlab_token

if exist %BACKUP_DIR% (
    echo "Removing existing backup directory..."
    rmdir /s /q %BACKUP_DIR%
)

echo "Creating backup directory..."
mkdir %BACKUP_DIR%

cd %BACKUP_DIR%
for /f "usebackq" %%i in (`curl -s --header "Private-Token: %GITLAB_TOKEN%" "%GITLAB_URL%/api/v4/projects?per_page=100" ^| jq -r ".[].ssh_url_to_repo"`) do (
    echo "Backing up %%i"
    git clone --mirror %%i
)

echo "Backup complete"