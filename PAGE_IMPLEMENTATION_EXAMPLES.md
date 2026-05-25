## Примеры реализации методов сохранения данных для каждой страницы

### 1. RelativesPage - Страница родственников

```csharp
private void AddRelativeButton_Click(object sender, RoutedEventArgs e)
{
    try
    {
        // Валидация
        if (string.IsNullOrWhiteSpace(LastNameTextBox.Text))
        {
            MessageBox.Show("Укажите фамилию", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        // Сохранение в БД
        int relativeId = DataService.CreateRelative(
            SessionManager.CurrentApplicantId.Value,
            InnTextBox.Text?.Trim(),
            RelationDegreeCombo.Text,
            LastNameTextBox.Text?.Trim(),
            FirstNameTextBox.Text?.Trim(),
            PatronymicTextBox.Text?.Trim(),
            BirthDatePicker.SelectedDate,
            PhoneTextBox.Text?.Trim(),
            EmailTextBox.Text?.Trim(),
            WorkPlaceTextBox.Text?.Trim(),
            PositionTextBox.Text?.Trim()
        );

        // Логирование
        DataService.LogChange("Relatives", relativeId, "CREATE");

        MessageBox.Show("Родственник добавлен", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
    }
}

public void LoadRelativesFromDatabase()
{
    try
    {
        var relatives = DataService.GetApplicantRelatives(SessionManager.CurrentApplicantId.Value);
        foreach (var relative in relatives)
        {
            RelativesCollection.Add(new RelativeViewModel
            {
                Id = relative.Id,
                LastName = relative.LastName,
                FirstName = relative.FirstName,
                Patronymic = relative.Patronymic
            });
        }
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Ошибка загрузки: {ex.Message}", "Ошибка");
    }
}
```

### 2. AdditionalInfoPage - Языки и достижения

```csharp
// Загрузка языков
private void LoadLanguagesFromDatabase()
{
    try
    {
        var languages = DataService.GetApplicantLanguages(SessionManager.CurrentApplicantId.Value);
        foreach (var lang in languages)
        {
            LanguagesList.Add(new LanguageViewModel
            {
                Id = lang.Id,
                LanguageId = lang.LanguageId.Value,
                LanguageLevelId = lang.LanguageLevelId.Value,
                IsPrimary = lang.IsPrimary.Value
            });
        }
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Ошибка загрузки языков: {ex.Message}", "Ошибка");
    }
}

// Добавление языка
private void AddLanguageButton_Click(object sender, RoutedEventArgs e)
{
    if (LanguageCombo.SelectedItem == null || LanguageLevelCombo.SelectedItem == null)
    {
        MessageBox.Show("Выберите язык и уровень", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
        return;
    }

    try
    {
        var language = LanguageCombo.SelectedItem as Languages;
        var level = LanguageLevelCombo.SelectedItem as LanguageLevels;
        bool isPrimary = IsPrimaryCheckBox.IsChecked ?? false;

        int langId = DataService.CreateApplicantLanguage(
            SessionManager.CurrentApplicantId.Value,
            language.Id,
            level.Id,
            isPrimary
        );

        DataService.LogChange("ApplicantLanguages", langId, "CREATE");

        LanguagesList.Add(new LanguageViewModel
        {
            Id = langId,
            LanguageId = language.Id,
            LanguageName = language.Name,
            LanguageLevelId = level.Id,
            LanguageLevelName = level.Name,
            IsPrimary = isPrimary
        });

        MessageBox.Show("Язык добавлен", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
    }
}

// Удаление языка
private void DeleteLanguageButton_Click(object sender, RoutedEventArgs e)
{
    var selected = LanguagesGrid.SelectedItem as LanguageViewModel;
    if (selected == null) return;

    try
    {
        DataService.DeleteApplicantLanguage(selected.Id);
        DataService.LogChange("ApplicantLanguages", selected.Id, "DELETE");
        LanguagesList.Remove(selected);
        MessageBox.Show("Язык удален", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
    }
}

// Загрузка спортивных достижений
private void LoadSportAchievementsFromDatabase()
{
    try
    {
        var achievements = DataService.GetApplicantSportAchievements(SessionManager.CurrentApplicantId.Value);
        foreach (var achievement in achievements)
        {
            SportsList.Add(new SportAchievementViewModel
            {
                Id = achievement.Id,
                SportType = achievement.SportType,
                Achievement = achievement.Achievement,
                Rank = achievement.Rank,
                Year = achievement.Year
            });
        }
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Ошибка загрузки: {ex.Message}", "Ошибка");
    }
}

// Добавление спортивного достижения
private void AddSportAchievementButton_Click(object sender, RoutedEventArgs e)
{
    if (string.IsNullOrWhiteSpace(SportTypeTextBox.Text) || string.IsNullOrWhiteSpace(AchievementTextBox.Text))
    {
        MessageBox.Show("Заполните все поля", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
        return;
    }

    try
    {
        int achievementId = DataService.CreateSportAchievement(
            SessionManager.CurrentApplicantId.Value,
            SportTypeTextBox.Text,
            AchievementTextBox.Text,
            RankTextBox.Text,
            int.TryParse(YearTextBox.Text, out int year) ? year : (int?)null
        );

        DataService.LogChange("SportAchievements", achievementId, "CREATE");

        SportsList.Add(new SportAchievementViewModel
        {
            Id = achievementId,
            SportType = SportTypeTextBox.Text,
            Achievement = AchievementTextBox.Text,
            Rank = RankTextBox.Text,
            Year = int.TryParse(YearTextBox.Text, out int y) ? y : (int?)null
        });

        ClearSportForm();
        MessageBox.Show("Достижение добавлено", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
    }
}
```

### 3. IndividualAchievementsPage - Индивидуальные достижения

```csharp
// Загрузка индивидуальных достижений
private void LoadIndividualAchievementsFromDatabase()
{
    try
    {
        var achievements = DataService.GetApplicantIndividualAchievements(SessionManager.CurrentApplicantId.Value);
        foreach (var achievement in achievements)
        {
            AchievementsList.Add(new IndividualAchievementViewModel
            {
                Id = achievement.Id,
                Achievement = achievement.Achievement
            });
        }
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Ошибка загрузки: {ex.Message}", "Ошибка");
    }
}

// Добавление индивидуального достижения
private void AddAchievementButton_Click(object sender, RoutedEventArgs e)
{
    if (string.IsNullOrWhiteSpace(AchievementTextBox.Text))
    {
        MessageBox.Show("Укажите достижение", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
        return;
    }

    try
    {
        int achievementId = DataService.CreateIndividualAchievement(
            SessionManager.CurrentApplicantId.Value,
            AchievementTextBox.Text
        );

        DataService.LogChange("IndividualAchievements", achievementId, "CREATE");

        AchievementsList.Add(new IndividualAchievementViewModel
        {
            Id = achievementId,
            Achievement = AchievementTextBox.Text
        });

        AchievementTextBox.Clear();
        MessageBox.Show("Достижение добавлено", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
    }
}

// Удаление индивидуального достижения
private void DeleteAchievementButton_Click(object sender, RoutedEventArgs e)
{
    var selected = AchievementsGrid.SelectedItem as IndividualAchievementViewModel;
    if (selected == null) return;

    try
    {
        DataService.DeleteIndividualAchievement(selected.Id);
        DataService.LogChange("IndividualAchievements", selected.Id, "DELETE");
        AchievementsList.Remove(selected);
        MessageBox.Show("Достижение удалено", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
    }
}
```

### 4. PrioritiesPage - Приоритеты программ

```csharp
// Загрузка приоритетов
private void LoadPrioritiesFromDatabase()
{
    try
    {
        var priorities = DataService.GetApplicantPriorities(SessionManager.CurrentApplicantId.Value);
        foreach (var priority in priorities)
        {
            PrioritiesList.Add(new ApplicationPriorityViewModel
            {
                Id = priority.Id,
                PriorityOrder = priority.PriorityOrder,
                ProgramCode = priority.ProgramCode,
                ProgramName = priority.ProgramName,
                StudyForm = priority.StudyForm,
                EducationBase = priority.EducationBase,
                Department = priority.Department,
                AdmissionType = priority.AdmissionType,
                Branch = priority.Branch,
                IsSelected = priority.IsSelected ?? false
            });
        }
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Ошибка загрузки: {ex.Message}", "Ошибка");
    }
}

// Добавление приоритета
private void AddPriorityButton_Click(object sender, RoutedEventArgs e)
{
    if (string.IsNullOrWhiteSpace(ProgramNameTextBox.Text))
    {
        MessageBox.Show("Укажите название программы", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
        return;
    }

    try
    {
        int nextOrder = PrioritiesList.Count + 1;

        int priorityId = DataService.CreateApplicationPriority(
            SessionManager.CurrentApplicantId.Value,
            nextOrder,
            ProgramCodeTextBox.Text,
            ProgramNameTextBox.Text,
            StudyFormCombo.Text,
            EducationBaseCombo.Text,
            DepartmentTextBox.Text,
            AdmissionTypeCombo.Text,
            BranchTextBox.Text
        );

        DataService.LogChange("ApplicationPriorities", priorityId, "CREATE");

        PrioritiesList.Add(new ApplicationPriorityViewModel
        {
            Id = priorityId,
            PriorityOrder = nextOrder,
            ProgramCode = ProgramCodeTextBox.Text,
            ProgramName = ProgramNameTextBox.Text,
            StudyForm = StudyFormCombo.Text,
            EducationBase = EducationBaseCombo.Text,
            Department = DepartmentTextBox.Text,
            AdmissionType = AdmissionTypeCombo.Text,
            Branch = BranchTextBox.Text
        });

        ClearPriorityForm();
        MessageBox.Show("Приоритет добавлен", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
    }
}

// Удаление приоритета
private void DeletePriorityButton_Click(object sender, RoutedEventArgs e)
{
    var selected = PrioritiesGrid.SelectedItem as ApplicationPriorityViewModel;
    if (selected == null) return;

    try
    {
        DataService.DeleteApplicationPriority(selected.Id);
        DataService.LogChange("ApplicationPriorities", selected.Id, "DELETE");
        PrioritiesList.Remove(selected);
        MessageBox.Show("Приоритет удален", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
    }
}
```

### 5. AttachedDocumentsPage - Прикрепленные документы

```csharp
// Загрузка прикрепленных документов
private void LoadAttachedDocumentsFromDatabase()
{
    try
    {
        var documents = DataService.GetApplicantAttachedDocuments(SessionManager.CurrentApplicantId.Value);
        foreach (var doc in documents)
        {
            AttachedDocumentsList.Add(new AttachedDocumentViewModel
            {
                Id = doc.Id,
                DocumentName = doc.DocumentName,
                DocumentType = doc.DocumentType,
                FilePath = doc.FilePath,
                FileSize = doc.FileSize,
                UploadedAt = doc.UploadedAt
            });
        }
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Ошибка загрузки: {ex.Message}", "Ошибка");
    }
}

// Добавление документа
private void AttachDocumentButton_Click(object sender, RoutedEventArgs e)
{
    var openFileDialog = new OpenFileDialog();
    if (openFileDialog.ShowDialog() == true)
    {
        try
        {
            string filePath = openFileDialog.FileName;
            string fileName = Path.GetFileName(filePath);
            string fileType = Path.GetExtension(filePath).TrimStart('.');

            var fileInfo = new FileInfo(filePath);
            int fileSize = (int)fileInfo.Length;

            int docId = DataService.CreateAttachedDocument(
                SessionManager.CurrentApplicantId.Value,
                fileName,
                fileType,
                filePath,
                fileSize
            );

            DataService.LogChange("AttachedDocuments", docId, "CREATE");

            AttachedDocumentsList.Add(new AttachedDocumentViewModel
            {
                Id = docId,
                DocumentName = fileName,
                DocumentType = fileType,
                FilePath = filePath,
                FileSize = fileSize,
                UploadedAt = DateTime.Now
            });

            MessageBox.Show("Документ прикреплен", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}

// Удаление документа
private void DeleteDocumentButton_Click(object sender, RoutedEventArgs e)
{
    var selected = DocumentsGrid.SelectedItem as AttachedDocumentViewModel;
    if (selected == null) return;

    try
    {
        DataService.DeleteAttachedDocument(selected.Id);
        DataService.LogChange("AttachedDocuments", selected.Id, "DELETE");
        AttachedDocumentsList.Remove(selected);
        MessageBox.Show("Документ удален", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
    }
}
```

### 6. ApplicationCompetitionsPage - Конкурсы

```csharp
// Загрузка конкурсов
private void LoadCompetitionsFromDatabase()
{
    try
    {
        var competitions = DataService.GetApplicantCompetitions(SessionManager.CurrentApplicantId.Value);
        foreach (var comp in competitions)
        {
            CompetitionsList.Add(new CompetitionPriorityViewModel
            {
                Id = comp.Id,
                CompetitionName = comp.CompetitionName,
                PriorityOrder = comp.PriorityOrder,
                IsSelected = comp.IsSelected ?? false
            });
        }
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Ошибка загрузки: {ex.Message}", "Ошибка");
    }
}

// Добавление конкурса
private void AddCompetitionButton_Click(object sender, RoutedEventArgs e)
{
    if (string.IsNullOrWhiteSpace(CompetitionNameTextBox.Text))
    {
        MessageBox.Show("Укажите название конкурса", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
        return;
    }

    try
    {
        int nextOrder = CompetitionsList.Count + 1;

        int compId = DataService.CreateCompetitionPriority(
            SessionManager.CurrentApplicantId.Value,
            CompetitionNameTextBox.Text,
            nextOrder
        );

        DataService.LogChange("CompetitionPriorities", compId, "CREATE");

        CompetitionsList.Add(new CompetitionPriorityViewModel
        {
            Id = compId,
            CompetitionName = CompetitionNameTextBox.Text,
            PriorityOrder = nextOrder,
            IsSelected = false
        });

        CompetitionNameTextBox.Clear();
        MessageBox.Show("Конкурс добавлен", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
    }
}

// Удаление конкурса
private void DeleteCompetitionButton_Click(object sender, RoutedEventArgs e)
{
    var selected = CompetitionsGrid.SelectedItem as CompetitionPriorityViewModel;
    if (selected == null) return;

    try
    {
        DataService.DeleteCompetitionPriority(selected.Id);
        DataService.LogChange("CompetitionPriorities", selected.Id, "DELETE");
        CompetitionsList.Remove(selected);
        MessageBox.Show("Конкурс удален", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
    }
    catch (Exception ex)
    {
        MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
    }
}
```

## Общие рекомендации

1. **Всегда проверяйте SessionManager.CurrentApplicantId** перед операциями с БД
2. **Логируйте все операции** через `DataService.LogChange()`
3. **Используйте try-catch** для обработки ошибок БД
4. **Обновляйте UI** после успешного сохранения
5. **Загружайте данные** из БД при инициализации страницы
6. **Используйте ObservableCollection** для привязки данных в XAML
