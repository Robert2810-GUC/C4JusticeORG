# Ubuntu DigitalOcean Server — Complete Management Guide

> **Server IP:** `68.183.131.252`  
> **Auth:** Password  
> **OS:** Ubuntu (nginx + .NET + MySQL)

---

## 1. Connecting via SSH

### Windows (PowerShell / Command Prompt)
```bash
ssh root@68.183.131.252
```
- Type `yes` when asked to trust the fingerprint
- Enter your password when prompted (characters won't show — that's normal)

### Windows (PuTTY — if you prefer a GUI)
1. Download PuTTY from https://putty.org
2. Open PuTTY → Host Name: `68.183.131.252` → Port: `22` → Connection type: SSH
3. Click **Open** → login as: `root` → enter password

### Stay connected (prevent timeout)
```bash
# Add to ~/.ssh/config on your LOCAL machine
Host digitalocean
    HostName 68.183.131.252
    User root
    ServerAliveInterval 60
```
Then connect with: `ssh digitalocean`

---

## 2. Basic Linux Commands

### Navigation
```bash
pwd                        # show current directory
ls                         # list files
ls -la                     # list files with details + hidden files
cd /var/www/app1           # go to app folder
cd ..                      # go up one level
cd ~                       # go to home directory
```

### Files & Folders
```bash
mkdir myfolder             # create folder
rm myfile.txt              # delete file
rm -rf myfolder            # delete folder and everything in it (careful!)
cp file.txt /path/copy.txt # copy file
mv file.txt /path/new.txt  # move or rename file
touch newfile.txt          # create empty file
cat file.txt               # print file contents
less file.txt              # scroll through file (q to quit)
nano file.txt              # edit file in terminal
```

### Search
```bash
find / -name "nginx.conf"  # find a file by name
grep -r "client_max" /etc/nginx/   # search text inside files
```

### System Info
```bash
df -h                      # disk space usage
free -h                    # RAM usage
top                        # live process monitor (q to quit)
htop                       # better process monitor (install: apt install htop)
uptime                     # how long server has been running
whoami                     # current user
```

### Network
```bash
curl ifconfig.me           # show public IP
ss -tlnp                   # show open ports and services
ping google.com            # test internet connection
```

---

## 3. File Permissions
```bash
chmod 755 file.sh          # rwx for owner, rx for others
chmod 644 file.txt         # rw for owner, r for others
chown www-data:www-data /var/www/app1   # change file owner
```

---

## 4. Service Management (systemctl)

```bash
# Your .NET app
sudo systemctl status app1.service     # check if app is running
sudo systemctl start app1.service      # start app
sudo systemctl stop app1.service       # stop app
sudo systemctl restart app1.service    # restart app
sudo systemctl enable app1.service     # auto-start on reboot

# nginx
sudo systemctl status nginx
sudo systemctl restart nginx
sudo systemctl reload nginx            # reload config without downtime

# MySQL
sudo systemctl status mysql
sudo systemctl restart mysql
```

### View app logs (live)
```bash
sudo journalctl -u app1.service -f          # live logs for your .NET app
sudo journalctl -u app1.service -n 100      # last 100 lines
sudo journalctl -u nginx -f                  # nginx logs live
```

---

## 5. nginx Configuration

### Key file locations
```
/etc/nginx/nginx.conf                  # main config
/etc/nginx/sites-available/default    # your site config
/etc/nginx/sites-enabled/             # active sites (symlinks)
/var/log/nginx/access.log             # access log
/var/log/nginx/error.log              # error log
```

### Edit site config
```bash
sudo nano /etc/nginx/sites-available/default
```

### Fix 413 Request Entity Too Large (your current issue)
Add this inside the `server { }` block:
```nginx
server {
    listen 80;
    server_name cu4justice.org;

    client_max_body_size 50M;   # <-- ADD THIS LINE

    location / {
        proxy_pass http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection keep-alive;
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
        proxy_read_timeout 300s;
        proxy_connect_timeout 300s;
        proxy_send_timeout 300s;
    }
}
```

### Apply config changes
```bash
sudo nginx -t                          # test config for errors FIRST
sudo systemctl reload nginx            # apply without downtime
```

### One-liner to fix 413 and reload
```bash
sudo sed -i '/server_name/a\    client_max_body_size 50M;' /etc/nginx/sites-available/default && sudo nginx -t && sudo systemctl reload nginx
```

---

## 6. MySQL Commands

### Connect to MySQL
```bash
mysql -u root -p           # login as root (will prompt for password)
```

### Inside MySQL shell
```sql
SHOW DATABASES;                        -- list all databases
USE your_database_name;                -- switch to your DB
SHOW TABLES;                           -- list tables
DESCRIBE VoterData;                    -- show table columns

-- Basic queries
SELECT COUNT(*) FROM VoterData;
SELECT * FROM VoterData LIMIT 10;
SELECT * FROM VoterData WHERE AsOfDate = '2024-01-01';

-- Delete data
DELETE FROM VoterData WHERE AsOfDate = '2024-01-01';

-- Check DB size
SELECT table_schema AS "Database",
       ROUND(SUM(data_length + index_length) / 1024 / 1024, 2) AS "Size (MB)"
FROM information_schema.TABLES
GROUP BY table_schema;

-- Exit
EXIT;
```

### Backup & Restore
```bash
# Backup entire database
mysqldump -u root -p your_database_name > backup_$(date +%Y%m%d).sql

# Restore from backup
mysql -u root -p your_database_name < backup_20240101.sql

# Backup and compress
mysqldump -u root -p your_database_name | gzip > backup_$(date +%Y%m%d).sql.gz
```

---

## 7. .NET App Management

### App file location
```bash
ls /var/www/app1/publish/              # your deployed app files
```

### Check if app is running
```bash
sudo systemctl status app1.service
curl http://localhost:5000              # test app responds locally
```

### View appsettings (connection strings)
```bash
cat /var/www/app1/publish/appsettings.json
nano /var/www/app1/publish/appsettings.Production.json   # edit prod settings
```

### Manual restart after deploy
```bash
sudo systemctl restart app1.service && sudo systemctl status app1.service
```

---

## 8. FileZilla (GUI File Transfer)

### Connect to server
1. Open FileZilla
2. Go to **File → Site Manager → New Site**
3. Fill in:
   - **Protocol:** SFTP - SSH File Transfer Protocol
   - **Host:** `68.183.131.252`
   - **Port:** `22`
   - **Logon Type:** Normal
   - **User:** `root`
   - **Password:** your password
4. Click **Connect**

### Key server paths to navigate to
| Purpose | Path |
|---|---|
| App files | `/var/www/app1/publish/` |
| nginx config | `/etc/nginx/sites-available/` |
| Upload folder | `/var/www/app1/publish/wwwroot/uploads/` |
| Logs | `/var/log/nginx/` |

### Upload a file via FileZilla
1. Left panel = your local computer
2. Right panel = server
3. Navigate to target folder on both sides
4. Drag file from left to right to upload

> **Tip:** Right-click a file on the server → **View/Edit** to edit it directly and auto-upload on save.

---

## 9. SSL Certificate (HTTPS)

### Install/Renew Let's Encrypt (free SSL)
```bash
sudo apt install certbot python3-certbot-nginx -y
sudo certbot --nginx -d cu4justice.org -d www.cu4justice.org
sudo certbot renew --dry-run          # test auto-renewal
```

---

## 10. Server Updates & Packages

```bash
sudo apt update                        # refresh package list
sudo apt upgrade -y                    # install all updates
sudo apt install htop curl git -y      # install specific packages
sudo apt autoremove -y                 # clean up unused packages
```

---

## 11. Useful One-Liners

```bash
# Watch nginx error log live
sudo tail -f /var/log/nginx/error.log

# Watch app log live
sudo journalctl -u app1.service -f

# Check what's using port 80
sudo lsof -i :80

# Kill a process by port
sudo fuser -k 5000/tcp

# Check disk usage of a folder
du -sh /var/www/app1/publish/

# Reboot the server
sudo reboot

# Show all environment variables for app
sudo systemctl show app1.service --property=Environment
```

---

## 12. Emergency Recovery

```bash
# App crashed? Check why:
sudo journalctl -u app1.service -n 50 --no-pager

# nginx not starting? Check config:
sudo nginx -t

# Can't connect to MySQL?
sudo systemctl start mysql
sudo journalctl -u mysql -n 30

# Out of disk space?
df -h
du -sh /var/log/*       # logs are often the culprit
sudo truncate -s 0 /var/log/nginx/access.log   # clear access log
```

---

## Quick Reference Card

| Task | Command |
|---|---|
| SSH in | `ssh root@68.183.131.252` |
| Restart app | `sudo systemctl restart app1.service` |
| Reload nginx | `sudo systemctl reload nginx` |
| Test nginx config | `sudo nginx -t` |
| Edit nginx site | `sudo nano /etc/nginx/sites-available/default` |
| Live app logs | `sudo journalctl -u app1.service -f` |
| MySQL login | `mysql -u root -p` |
| Check disk | `df -h` |
| Reboot | `sudo reboot` |
