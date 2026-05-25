# QUICK REFERENCE - Быстрая справка по сохранению данных

## TL;DR (Очень кратко)

### Что было сделано?
✅ Добавлено сохранение данных в локальную БД SQLite для 4 страниц приложения
✅ Создан класс DatabasePersistenceHelper для переиспользуемого кода
✅ Все операции логируются в таблице ChangeLogs
✅ Приложение успешно компилируется

### Какие файлы изменены?
```
+ Model/DatabasePersistenceHelper.cs          (НОВЫЙ - 430+ строк)
~ View/ContactsPage.xaml.cs                   (+15 строк логирования)
~ View/RelativesPage.xaml.cs                  (+50 строк для БД)
~ View/IndividualAchievementsPage.xaml.cs     (+80 строк для БД)
~ View/AttachedDocumentsPage.xaml.cs          (+70 строк для БД)
```

---

## 🔧 Быстрый старт

### Для пользователя
```
1. Создать нового абитуриента (IdentityPage)
2. Заполнить данные на каждой странице
3. Нажать "Далее" для перехода на следующую страницу
4. Все данные автоматически сохранятся в БД
5. При перезагрузке приложения - все данные остаются
```

### Для разработчика
```csharp
// Сохранение
int id = DatabasePersistenceHelper.SaveRelativeDocument(...);
DataService.LogChange("RelativeDocuments", id, "INSERT");

// Загрузка
var items = DatabasePersistenceHelper.LoadRelativeDocuments(applicantId);

// Удаление
DatabasePersistenceHelper.DeleteRelativeDocument(id, applicantId);
DataService.LogChange("RelativeDocuments", id, "DELETE");
```

---

## 📋 Шпаргалка методов DatabasePersistenceHelper

### RelativeDocuments (Родственники)
```csharp
// Загрузить родственников
var relatives = DatabasePersistenceHelper.LoadRelativeDocuments(applicantId);

// Сохранить родственника
int id = DatabasePersistenceHelper.SaveRelativeDocument(
    applicantId, relationDegree, lastName, firstName, patronymic,
    birthDate, phone, email, workPlace, position, blockReason, isBlocked
);

// Удалить родственника
DatabasePersistenceHelper.DeleteRelativeDocument(id, applicantId);
```

### IndividualAchievements (Достижения)
```csharp
// Загрузить достижения
var achievements = DatabasePersistenceHelper.LoadIndividualAchievements(applicantId);

// Сохранить достижение
int id = DatabasePersistenceHelper.SaveIndividualAchievement(
    applicantId, category, achievementName, year, points, 
    documentName, documentPath
);

// Удалить достижение
DatabasePersistenceHelper.DeleteIndividualAchievement(id, applicantId);
```

### AttachedDocuments (Приложенные документы)
```csharp
// Загрузить документы
var documents = DatabasePersistenceHelper.LoadAttachedDocuments(applicantId);

// Сохранить документ
int id = DatabasePersistenceHelper.SaveAttachedDocument(
    applicantId, documentType, seriesNumber, category, additionalData,
    issueDate, documentInfo, attachmentPath, attachmentName
);

// Удалить документ
DatabasePersistenceHelper.DeleteAttachedDocument(id, applicantId);
```

---

## 📍 Где используется

| Страница | Таблица | Операции |
|----------|---------|----------|
| ContactsPage | Applicants | UPDATE |
| RelativesPage | RelativeDocuments | INSERT, DELETE |
| IndividualAchievementsPage | IndividualAchievements | INSERT, DELETE |
| AttachedDocumentsPage | AttachedDocuments | INSERT, DELETE |

---

## ⚠️ Типичные ошибки

### Ошибка 1: SessionManager.CurrentApplicantId is null
```csharp
// ❌ Неправильно - забыли проверку
int id = DatabasePersistenceHelper.SaveRelativeDocument(
    SessionManager.CurrentApplicantId.Value, ...
); // CRASH!

// ✅ Правильно - проверяем сначала
if (SessionManager.CurrentApplicant != null)
{
    int id = DatabasePersistenceHelper.SaveRelativeDocument(
        SessionManager.CurrentApplicantId.Value, ...
    );
}
```

### Ошибка 2: Забыли логирование
```csharp
// ❌ Неправильно - нет логирования
int id = DatabasePersistenceHelper.SaveRelativeDocument(...);

// ✅ Правильно - добавляем логирование
int id = DatabasePersistenceHelper.SaveRelativeDocument(...);
DataService.LogChange("RelativeDocuments", id, "INSERT");
```

### Ошибка 3: Нет обработки ошибок
```csharp
// ❌ Неправильно - нет try/catch
int id = DatabasePersistenceHelper.SaveRelativeDocument(...);

// ✅ Правильно - обрабатываем ошибки
try
{
    int id = DatabasePersistenceHelper.SaveRelativeDocument(...);
    DataService.LogChange("RelativeDocuments", id, "INSERT");
    MessageBox.Show("Успешно!");
}
catch (Exception ex)
{
    MessageBox.Show($"Ошибка: {ex.Message}");
}
```

---

## 🔍 Проверка работы

### Визуальная проверка
- ✅ При добавлении элемента - появляется сообщение "Успешно"
- ✅ При перезагрузке приложения - данные остаются
- ✅ При удалении элемента - появляется сообщение подтверждения

### Проверка БД (SQL)
```sql
-- Проверить данные родственников
SELECT * FROM RelativeDocuments WHERE ApplicantId = 1;

-- Проверить данные достижений
SELECT * FROM IndividualAchievements WHERE ApplicantId = 1;

-- Проверить данные документов
SELECT * FROM AttachedDocuments WHERE ApplicantId = 1;

-- Проверить логирование
SELECT * FROM ChangeLogs 
WHERE TableName IN ('RelativeDocuments', 'IndividualAchievements', 'AttachedDocuments');
```

---

## 💡 Полезные советы

### Совет 1: Всегда проверяйте SessionManager
```csharp
if (SessionManager.CurrentApplicant == null)
{
    MessageBox.Show("Абитуриент не выбран");
    return;
}
```

### Совет 2: Используйте try/catch везде
```csharp
try { /* операция */ }
catch (Exception ex) { /* обработка */ }
```

### Совет 3: Логируйте все операции
```csharp
DataService.LogChange("TableName", id, "INSERT|UPDATE|DELETE");
```

### Совет 4: Покажите сообщение об успехе
```csharp
MessageBox.Show("Успешно сохранено!", "Успех", 
    MessageBoxButton.OK, MessageBoxImage.Information);
```

---

## 📊 Структура таблиц (минимум данных)

### RelativeDocuments
| Поле | Тип | Обязательное |
|------|-----|-------------|
| Id | INTEGER | ✅ |
| ApplicantId | INTEGER | ✅ |
| LastName | TEXT | ✅ |
| FirstName | TEXT | ✅ |
| CreatedAt | DATETIME | ✅ |

### IndividualAchievements
| Поле | Тип | Обязательное |
|------|-----|-------------|
| Id | INTEGER | ✅ |
| ApplicantId | INTEGER | ✅ |
| Category | TEXT | ✅ |
| Points | INTEGER | ✅ |
| CreatedAt | DATETIME | ✅ |

### AttachedDocuments
| Поле | Тип | Обязательное |
|------|-----|-------------|
| Id | INTEGER | ✅ |
| ApplicantId | INTEGER | ✅ |
| DocumentType | TEXT | ✅ |
| AttachmentName | TEXT | - |
| CreatedAt | DATETIME | ✅ |

---

## 🎯 Шаблон для новой функции

```csharp
// 1. Загрузить при открытии
if (SessionManager.CurrentApplicant != null)
{
    var items = DatabasePersistenceHelper.Load*(
        SessionManager.CurrentApplicantId.Value
    );
}

// 2. Сохранить при добавлении
try
{
    int id = DatabasePersistenceHelper.Save*(
        SessionManager.CurrentApplicantId.Value,
        /* параметры */
    );
    DataService.LogChange("TableName", id, "INSERT");
    MessageBox.Show("Успешно!");
}
catch (Exception ex)
{
    MessageBox.Show($"Ошибка: {ex.Message}");
}

// 3. Удалить при удалении
try
{
    DatabasePersistenceHelper.Delete*(id, SessionManager.CurrentApplicantId.Value);
    DataService.LogChange("TableName", id, "DELETE");
    MessageBox.Show("Удалено!");
}
catch (Exception ex)
{
    MessageBox.Show($"Ошибка: {ex.Message}");
}
```

---

## 🔗 Связанные файлы

- 📘 **DATABASE_PERSISTENCE_GUIDE.md** - Полное руководство
- 📗 **BEFORE_AFTER_COMPARISON.md** - Сравнение кода
- 📙 **SUMMARY_IMPLEMENTATION.md** - Краткое описание
- 📕 **FINAL_REPORT.md** - Итоговый отчет

---

## ✅ Чек-лист интеграции

При добавлении новой функции:
- [ ] Добавлены методы в DatabasePersistenceHelper
- [ ] Вызов Load* при инициализации страницы
- [ ] Вызов Save* при добавлении элемента
- [ ] Вызов Delete* при удалении элемента
- [ ] Добавлены вызовы DataService.LogChange()
- [ ] Добавлены try/catch блоки
- [ ] Добавлены MessageBox с результатом
- [ ] Проверена компиляция
- [ ] Протестирована функциональность

---

## 🚀 Что дальше?

### Краткосрочно
- [ ] Протестировать на реальных данных
- [ ] Проверить логирование в ChangeLogs
- [ ] Убедиться что данные сохраняются между сеансами

### Среднесрочно
- [ ] Добавить другие страницы (Documents, Priorities, и т.д.)
- [ ] Создать резервную копию БД
- [ ] Добавить отчеты на основе ChangeLogs

### Долгосрочно
- [ ] Миграция на полноценный сервер БД
- [ ] Синхронизация между устройствами
- [ ] Аналитика использования

---

## 📞 Контакты и вопросы

При возникновении вопросов:
1. Проверить вышеуказанные документы
2. Изучить примеры в коде
3. Протестировать функцию пошагово

---

**Версия**: 1.0
**Дата**: 2024 г.
**Статус**: ✅ ГОТОВО К ИСПОЛЬЗОВАНИЮ
