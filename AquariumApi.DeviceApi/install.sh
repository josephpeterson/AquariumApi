#!/bin/bash
sudo cp aquarium-device.service /etc/systemd/system
sudo systemctl daemon-reload
sudo systemctl enable aquarium-device