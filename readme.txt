#Installing dotnet core on Raspian
#https://blogs.msdn.microsoft.com/david/2017/07/20/setting_up_raspian_and_dotnet_core_2_0_on_a_raspberry_pi/
sudo apt-get install curl libunwind8 gettext
curl -sSL -o dotnet.tar.gz https://dotnetcli.blob.core.windows.net/dotnet/Runtime/release/2.0.0/dotnet-runtime-latest-linux-arm.tar.gz
sudo mkdir -p /opt/dotnet && sudo tar zxf dotnet.tar.gz -C /opt/dotnet
sudo ln -s /opt/dotnet/dotnet /usr/local/bin

#Running DeviceApi on boot
sudo cp launch.sh /etc/init.d/c_start_aquarium_deviceApi
sudo chmod 755 /etc/init.d/c_start_aquarium_deviceApi
sudo update-rc.d c_start_aquarium_deviceApi defaults

#3.5in Screen installation
sudo rm -rf LCD-show
git clone https://github.com/goodtft/LCD-show.git
chmod -R 755 LCD-show
cd LCD-show/
sudo ./LCD35-show
