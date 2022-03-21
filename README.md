# LibraryProject
  
Тестовый проект "Библиотека".  
  

Поддерживаются следующие операции:
-Добавить клиента.  
-Добавить Книгу.  
-Редактировать описание книги.  
-Выдача\возврат книги.  
  
  
Используется:  
-ASP.Net MVC Framework 4.7.2.  
-jquery 3.6.0  
-PostgreSQL  
-Docker  
  
  
При сборке контейнера базы данных происходит заполнения базы таблицами.  
Перед сборкой контейнера приложения должен быть сделан деплой в каталог.  
Для запуска Docker-Compose должны поддерживаться одновременно Windows и Linux-контейнеры.  
Сборка контейнера приложения требует загрузки официального контейнера Asp.Net:3.7.2 (~7 GB).  
Docker-Compose поднимает готовое приложение, для которого 80 порт пробрасывается в хостовую систему, контейнер с базой данных не имеет проброшенных портов.  
Строка подключения к базе хранится в отдельном файле, копируется при сборке контейнера, путь к файлу передается посредством переменных окружения.  
