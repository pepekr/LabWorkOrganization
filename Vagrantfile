# -*- mode: ruby -*-
# vi: set ft=ruby :

Vagrant.configure("2") do |config|
  
  # 1. Образ (Box)
  config.vm.box = "ubuntu/jammy64"

  # 2. Мережа (Network)
  # Ваш додаток буде доступний на http://192.168.56.10
  # Ми прокидаємо порт 80 (Nginx) з VM на порт 8080 вашого комп'ютера
  # для уникнення конфліктів.
  config.vm.network "private_network", ip: "192.168.56.10"
  config.vm.network "forwarded_port", guest: 80, host: 8080

  # 3. Скрипт налаштування (Provisioning)
  # Ми запускаємо той самий 'provision.sh', що й минулого разу
  config.vm.provision "shell", path: "provision.sh"

  # 4. Оптимізація (даємо машині більше пам'яті)
  config.vm.provider "virtualbox" do |vb|
	vb.gui = true
    vb.memory = "4096" # 4GB RAM
    vb.cpus = 2
  end
end