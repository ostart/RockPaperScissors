# RockPaperScissors

Проект предсталяет собой игру "Камень-Ножницы-Бумага" и реализован в виде WebAPI на NET6.

Скачать проект можно командой **git clone https://github.com/ostart/RockPaperScissors.git**

Для сборки проекта перейдите в терминале в папку **"RockPaperScissors\RockPaperScissors"** и выполните команду **dotnet build**

Для запуска проекта в терминале в папке **"RockPaperScissors\RockPaperScissors"** выполните команду **dotnet run**

Затем в браузере можно увидеть список доспупных запросов по адресу: http://localhost:5184/swagger/index.html

Сценарий использования игры:

1. Создать игру POST запросом /game/create?userName1=<ИмяИгрока>

2. Присоединить к игре второго игрока POST запросом /game/:gameId/join/:userName2

3. Сделать каждым игроком выбор хода POST запросом /game/:gameId/user/:userId/:turn

4. По окончанию игры получить статистику игры и имя победителя в результате GET запросом /game/:gameId/stat

