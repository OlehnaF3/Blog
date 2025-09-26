# ProjBlog
## Проект Блога

- Написан только бек с тестами
- Дефолтные вью с ошибками 
- Подключен сваггер и поддержкой токена

Для запуска сервиса нужно добавить файл app.json в корневую папку приложения

Пример файла

 ```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=blog.db" Строка подключения
  },
  "KeySettings": {
    "ISSUER": "MyAuthServer", Издатель токена
    "AUDIENCE": "MyAuthClient", Получатель токена
    "Key": "54a1fea6579fb9eed95bfaec5ce5d5f12c85823211f7af61419061ffc975e7f0", Секретный ключ
    "Time": "60" Время жизни токена 
  }
}
```


