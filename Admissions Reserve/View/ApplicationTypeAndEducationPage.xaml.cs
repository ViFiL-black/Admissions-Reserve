using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

namespace Admissions_Reserve.View
{
    public partial class ApplicationTypeAndEducationPage : Page
    {
        // Модель данных
        public class EducationDocumentInfo : INotifyPropertyChanged
        {
            private string _applicationType;
            private bool _firstTimeEducation;
            private string _country;
            private string _city;
            private string _educationalOrg;
            private string _documentType;
            private string _educationLevel;
            private string _documentEducationLevel;
            private string _series;
            private string _number;
            private DateTime? _issueDate;
            private DateTime? _graduationYear;
            private int _satisfactoryCount;
            private int _goodCount;
            private int _excellentCount;
            private double _averageScore;
            private bool _frdoVerified;
            private string _scanFilePath;
            private string _documentForm;
            private string _originalOrganization;

            public string ApplicationType { get => _applicationType; set { _applicationType = value; OnPropertyChanged(nameof(ApplicationType)); } }
            public bool FirstTimeEducation { get => _firstTimeEducation; set { _firstTimeEducation = value; OnPropertyChanged(nameof(FirstTimeEducation)); } }
            public string Country { get => _country; set { _country = value; OnPropertyChanged(nameof(Country)); } }
            public string City { get => _city; set { _city = value; OnPropertyChanged(nameof(City)); } }
            public string EducationalOrg { get => _educationalOrg; set { _educationalOrg = value; OnPropertyChanged(nameof(EducationalOrg)); } }
            public string DocumentType { get => _documentType; set { _documentType = value; OnPropertyChanged(nameof(DocumentType)); } }
            public string EducationLevel { get => _educationLevel; set { _educationLevel = value; OnPropertyChanged(nameof(EducationLevel)); } }
            public string DocumentEducationLevel { get => _documentEducationLevel; set { _documentEducationLevel = value; OnPropertyChanged(nameof(DocumentEducationLevel)); } }
            public string Series { get => _series; set { _series = value; OnPropertyChanged(nameof(Series)); } }
            public string Number { get => _number; set { _number = value; OnPropertyChanged(nameof(Number)); } }
            public DateTime? IssueDate { get => _issueDate; set { _issueDate = value; OnPropertyChanged(nameof(IssueDate)); } }
            public DateTime? GraduationYear { get => _graduationYear; set { _graduationYear = value; OnPropertyChanged(nameof(GraduationYear)); } }
            public int SatisfactoryCount { get => _satisfactoryCount; set { _satisfactoryCount = value; OnPropertyChanged(nameof(SatisfactoryCount)); CalculateAverageScore(); } }
            public int GoodCount { get => _goodCount; set { _goodCount = value; OnPropertyChanged(nameof(GoodCount)); CalculateAverageScore(); } }
            public int ExcellentCount { get => _excellentCount; set { _excellentCount = value; OnPropertyChanged(nameof(ExcellentCount)); CalculateAverageScore(); } }
            public double AverageScore { get => _averageScore; set { _averageScore = value; OnPropertyChanged(nameof(AverageScore)); } }
            public bool FrdoVerified { get => _frdoVerified; set { _frdoVerified = value; OnPropertyChanged(nameof(FrdoVerified)); } }
            public string ScanFilePath { get => _scanFilePath; set { _scanFilePath = value; OnPropertyChanged(nameof(ScanFilePath)); } }
            public string DocumentForm { get => _documentForm; set { _documentForm = value; OnPropertyChanged(nameof(DocumentForm)); } }
            public string OriginalOrganization { get => _originalOrganization; set { _originalOrganization = value; OnPropertyChanged(nameof(OriginalOrganization)); } }

            private void CalculateAverageScore()
            {
                int totalSubjects = SatisfactoryCount + GoodCount + ExcellentCount;
                if (totalSubjects > 0)
                {
                    AverageScore = (double)(SatisfactoryCount * 3 + GoodCount * 4 + ExcellentCount * 5) / totalSubjects;
                    AverageScore = Math.Round(AverageScore, 2);
                }
                else
                {
                    AverageScore = 0;
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;
            protected void OnPropertyChanged(string name) =>
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private EducationDocumentInfo _educationInfo;

        public ApplicationTypeAndEducationPage()
        {
            InitializeComponent();
            InitializeEvents();
            _educationInfo = new EducationDocumentInfo();
            DataContext = _educationInfo;
            LoadSampleData();
        }

        private void InitializeEvents()
        {
            // Привязка данных к модели
            ApplicationTypeCombo.SelectionChanged += (s, e) =>
                _educationInfo.ApplicationType = (ApplicationTypeCombo.SelectedItem as ComboBoxItem)?.Content.ToString();

            FirstTimeEducationCheckBox.Checked += (s, e) => _educationInfo.FirstTimeEducation = true;
            FirstTimeEducationCheckBox.Unchecked += (s, e) => _educationInfo.FirstTimeEducation = false;

            CountryCombo.SelectionChanged += (s, e) =>
                _educationInfo.Country = (CountryCombo.SelectedItem as ComboBoxItem)?.Content.ToString();

            CityTextBox.TextChanged += (s, e) => _educationInfo.City = CityTextBox.Text;
            EducationalOrgTextBox.TextChanged += (s, e) => _educationInfo.EducationalOrg = EducationalOrgTextBox.Text;

            DocumentTypeCombo.SelectionChanged += (s, e) =>
                _educationInfo.DocumentType = (DocumentTypeCombo.SelectedItem as ComboBoxItem)?.Content.ToString();

            EducationLevelCombo.SelectionChanged += (s, e) =>
                _educationInfo.EducationLevel = (EducationLevelCombo.SelectedItem as ComboBoxItem)?.Content.ToString();

            DocumentEducationLevelCombo.SelectionChanged += (s, e) =>
                _educationInfo.DocumentEducationLevel = (DocumentEducationLevelCombo.SelectedItem as ComboBoxItem)?.Content.ToString();

            SeriesTextBox.TextChanged += (s, e) => _educationInfo.Series = SeriesTextBox.Text;
            NumberTextBox.TextChanged += (s, e) => _educationInfo.Number = NumberTextBox.Text;

            IssueDatePicker.SelectedDateChanged += (s, e) => _educationInfo.IssueDate = IssueDatePicker.SelectedDate;
            GraduationYearPicker.SelectedDateChanged += (s, e) => _educationInfo.GraduationYear = GraduationYearPicker.SelectedDate;

            // Подсчет оценок
            SatisfactoryCountTextBox.TextChanged += (s, e) => UpdateCounts();
            GoodCountTextBox.TextChanged += (s, e) => UpdateCounts();
            ExcellentCountTextBox.TextChanged += (s, e) => UpdateCounts();

            DocumentFormCombo.SelectionChanged += (s, e) =>
                _educationInfo.DocumentForm = (DocumentFormCombo.SelectedItem as ComboBoxItem)?.Content.ToString();

            OriginalOrganizationTextBox.TextChanged += (s, e) => _educationInfo.OriginalOrganization = OriginalOrganizationTextBox.Text;
        }

        private void UpdateCounts()
        {
            int satisfactory = ParseInt(SatisfactoryCountTextBox.Text);
            int good = ParseInt(GoodCountTextBox.Text);
            int excellent = ParseInt(ExcellentCountTextBox.Text);

            _educationInfo.SatisfactoryCount = satisfactory;
            _educationInfo.GoodCount = good;
            _educationInfo.ExcellentCount = excellent;

            AverageScoreTextBox.Text = _educationInfo.AverageScore.ToString("0.00");
        }

        private int ParseInt(string value)
        {
            if (int.TryParse(value, out int result))
                return result;
            return 0;
        }

        private void LoadSampleData()
        {
            ApplicationTypeCombo.SelectedIndex = 0;
            FirstTimeEducationCheckBox.IsChecked = true;
            CountryCombo.SelectedIndex = 0;
            CityTextBox.Text = "Москва";
            EducationalOrgTextBox.Text = "ГБОУ Школа № 1234";
            DocumentTypeCombo.SelectedIndex = 0;
            EducationLevelCombo.SelectedIndex = 0;
            DocumentEducationLevelCombo.SelectedIndex = 0;
            SeriesTextBox.Text = "78";
            NumberTextBox.Text = "123456";
            IssueDatePicker.SelectedDate = new DateTime(2023, 6, 30);
            GraduationYearPicker.SelectedDate = new DateTime(2023, 6, 30);
            SatisfactoryCountTextBox.Text = "0";
            GoodCountTextBox.Text = "3";
            ExcellentCountTextBox.Text = "2";
            DocumentFormCombo.SelectedIndex = 0;
        }

        private void UploadScanButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Выберите файл скан-копии документа об образовании",
                Filter = "PDF файлы (*.pdf)|*.pdf|Изображения (*.jpg;*.png;*.jpeg)|*.jpg;*.png;*.jpeg|Все файлы (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                string fileName = System.IO.Path.GetFileName(openFileDialog.FileName);
                ScanFileTextBox.Text = fileName;
                _educationInfo.ScanFilePath = openFileDialog.FileName;
            }
        }

        private void CheckFrdoButton_Click(object sender, RoutedEventArgs e)
        {
            // Имитация проверки в ФРДО
            if (!string.IsNullOrWhiteSpace(SeriesTextBox.Text) && !string.IsNullOrWhiteSpace(NumberTextBox.Text))
            {
                var result = MessageBox.Show($"Проверить документ {SeriesTextBox.Text} {NumberTextBox.Text} в ФРДО?",
                    "Проверка в ФРДО", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    _educationInfo.FrdoVerified = true;
                    FrdoVerifiedCheckBox.IsChecked = true;
                    MessageBox.Show("Документ успешно проверен в ФРДО", "Результат проверки",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, заполните серию и номер документа", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // Получение данных из формы
        public EducationDocumentInfo GetEducationInfo()
        {
            return _educationInfo;
        }

        // Заполнение формы данными
        public void SetEducationInfo(EducationDocumentInfo info)
        {
            if (info == null) return;

            ApplicationTypeCombo.Text = info.ApplicationType;
            FirstTimeEducationCheckBox.IsChecked = info.FirstTimeEducation;
            CountryCombo.Text = info.Country;
            CityTextBox.Text = info.City;
            EducationalOrgTextBox.Text = info.EducationalOrg;
            DocumentTypeCombo.Text = info.DocumentType;
            EducationLevelCombo.Text = info.EducationLevel;
            DocumentEducationLevelCombo.Text = info.DocumentEducationLevel;
            SeriesTextBox.Text = info.Series;
            NumberTextBox.Text = info.Number;
            IssueDatePicker.SelectedDate = info.IssueDate;
            GraduationYearPicker.SelectedDate = info.GraduationYear;
            SatisfactoryCountTextBox.Text = info.SatisfactoryCount.ToString();
            GoodCountTextBox.Text = info.GoodCount.ToString();
            ExcellentCountTextBox.Text = info.ExcellentCount.ToString();
            FrdoVerifiedCheckBox.IsChecked = info.FrdoVerified;
            DocumentFormCombo.Text = info.DocumentForm;
            OriginalOrganizationTextBox.Text = info.OriginalOrganization;

            UpdateCounts();
        }

        // Валидация формы
        public bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(CityTextBox.Text))
            {
                MessageBox.Show("Пожалуйста, укажите населенный пункт", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                CityTextBox.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(EducationalOrgTextBox.Text))
            {
                MessageBox.Show("Пожалуйста, укажите образовательную организацию", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                EducationalOrgTextBox.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(NumberTextBox.Text))
            {
                MessageBox.Show("Пожалуйста, укажите номер документа", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                NumberTextBox.Focus();
                return false;
            }

            if (IssueDatePicker.SelectedDate == null)
            {
                MessageBox.Show("Пожалуйста, укажите дату выдачи документа", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (GraduationYearPicker.SelectedDate == null)
            {
                MessageBox.Show("Пожалуйста, укажите год окончания", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }
    }
}