#!/bin/bash
screen -d -m -S aquarium bash -c 'cd $HOME/Desktop/DeviceApi && dotnet AquariumApi.DeviceApi.dll'


[Unit]
Description=Aquarium Device API .NET

[Service]
WorkingDirectory=/var/DeviceApi/
ExecStart=/usr/local/bin/dotnet /var/DeviceApi/AquariumApi.DeviceApi.dll
Restart=always
# Restart service after 10 seconds if the dotnet service crashes:
RestartSec=10
SyslogIdentifier=dotnet-example
User=pi
Environment=ASPNETCORE_ENVIRONMENT=Production

[Install]
WantedBy=multi-user.target