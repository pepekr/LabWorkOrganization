#!/bin/bash

# --- 1. Оновлення пакетів ---
echo "--- Updating packages ---"
sudo apt-get update -y > /dev/null

# --- 2. Встановлення Docker ---
echo "--- Installing Docker ---"
sudo apt-get install -y docker.io
sudo systemctl enable docker
sudo systemctl start docker

# --- 3. Додавання користувача 'vagrant' до групи 'docker' ---
echo "--- Adding vagrant user to docker group ---"
sudo usermod -aG docker vagrant

# --- 4. Встановлення Docker Compose ---
echo "--- Installing Docker Compose ---"
sudo apt-get install -y docker-compose

# --- 5. Запуск проєкту ---
echo "--- Starting application with Docker Compose ---"
# Переходимо в папку проєкту
cd /vagrant

# Запускаємо docker-compose у фоновому режимі (-d)
# --build змусить його зібрати ваші нові Docker-файли
sudo docker-compose up -d --build

echo "---"
echo "--- Provisioning complete! ---"
echo "--- Your app should be available at http://192.168.56.10 ---"
echo "--- (Or http://localhost:8080 on your host machine) ---"
echo "---"