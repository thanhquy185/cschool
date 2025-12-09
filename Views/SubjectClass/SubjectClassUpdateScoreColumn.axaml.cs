using System;
using System.Linq;
using System.Reactive.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Utils;
using ViewModels;

namespace Views.SubjectClass
{
    public partial class SubjectClassUpdateScoreColumn : Window
    {
        public SubjectClassUpdateScoreColumn(SubjectClassViewModel vm)
        {
            InitializeComponent(); 
            DataContext = vm;
            
            // Load dữ liệu ban đầu
            if (vm.SelectedSubjectClass != null)
            {
                vm.QuizCountText = vm.SelectedSubjectClass.QuizCount.ToString();
                vm.OralCountText = vm.SelectedSubjectClass.OralCount.ToString();
            }
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            var quizTextBox = this.FindControl<TextBox>("QuizCount");
            if (quizTextBox != null)
            {
                quizTextBox.AddHandler(TextInputEvent, NumericTextInput, RoutingStrategies.Tunnel);
                quizTextBox.PropertyChanged += OnTextBoxPropertyChanged;
            }

            var oralTextBox = this.FindControl<TextBox>("OralCount");
            if (oralTextBox != null)
            {
                oralTextBox.AddHandler(TextInputEvent, NumericTextInput, RoutingStrategies.Tunnel);
                oralTextBox.PropertyChanged += OnTextBoxPropertyChanged;
            }
        }

        // Chặn ngay khi nhập ký tự không phải số
        private void NumericTextInput(object? sender, TextInputEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Text))
                return;
                
            // Chặn TẤT CẢ ký tự không phải số
            if (e.Text.Any(c => !char.IsDigit(c)))
            {
                e.Handled = true;
            }
        }

        // Backup: Loại bỏ ký tự không hợp lệ nếu vẫn lọt qua
        private void OnTextBoxPropertyChanged(object? sender, Avalonia.AvaloniaPropertyChangedEventArgs e)
        {
            if (e.Property.Name != nameof(TextBox.Text) || sender is not TextBox textBox)
                return;

            var text = textBox.Text ?? "";
            
            // Nếu có ký tự không phải số, loại bỏ ngay
            if (text.Any(c => !char.IsDigit(c)))
            {
                int caretIndex = textBox.CaretIndex;
                string cleanText = new string(text.Where(char.IsDigit).ToArray());
                
                // Tạm thời tắt event để tránh vòng lặp vô hạn
                textBox.PropertyChanged -= OnTextBoxPropertyChanged;
                textBox.Text = cleanText;
                textBox.PropertyChanged += OnTextBoxPropertyChanged;
                
                textBox.CaretIndex = Math.Min(caretIndex, cleanText.Length);
            }
        }

        private async void ConfirmButton_Click(object? sender, RoutedEventArgs e)
        {
            if (DataContext is not SubjectClassViewModel vm)
                return;

            string quizText = vm.QuizCountText?.Trim() ?? string.Empty;
            string oralText = vm.OralCountText?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(quizText) || string.IsNullOrWhiteSpace(oralText))
            {
                await MessageBoxUtil.ShowError("Vui lòng nhập đầy đủ số lượng bài kiểm tra và bài miệng.", owner: this);
                return;
            }

            if (!int.TryParse(quizText, out var quizCount) || !int.TryParse(oralText, out var oralCount))
            {
                await MessageBoxUtil.ShowError("Vui lòng nhập số hợp lệ cho số lượng bài kiểm tra và bài miệng.", owner: this);
                return;
            }

            var currentQuizCount = vm.SelectedSubjectClass?.QuizCount ?? 0;
            var currentOralCount = vm.SelectedSubjectClass?.OralCount ?? 0;

            if (quizCount < currentQuizCount || oralCount < currentOralCount)
            {
                await MessageBoxUtil.ShowError("Số lượng bài kiểm tra và bài miệng mới phải lớn hơn hoặc bằng số lượng hiện tại.", owner: this);
                return;
            }
            
            vm.SelectedSubjectClass!.QuizCount = quizCount;
            vm.SelectedSubjectClass!.OralCount = oralCount;
            await vm.UpdateScoreColumnsCommand.Execute().ToTask();
            Close(true);
        }

        private void OnCancelButtonClick(object? sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}