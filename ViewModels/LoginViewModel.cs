using System.Windows.Input;
using Masters_Summer_Project_CsharpPart2_Quiz.Helpers;
using Masters_Summer_Project_CsharpPart2_Quiz.Repositories;
using Masters_Summer_Project_CsharpPart2_Quiz.Services;
using Masters_Summer_Project_CsharpPart2_Quiz.Views;
using Masters_Summer_Project_CsharpPart2_Quiz.ViewModels.Presentations;

namespace Masters_Summer_Project_CsharpPart2_Quiz.ViewModels;
public partial class LoginViewModel : BaseViewModel
{
    public ICommand LoginCommand { get; private set; }
    public ICommand GoToRegisterCommand { get; private set; }
    private readonly UserService _userService;
    private UserProperty _userProperty;
    public UserProperty UserProperty
    {
        get => _userProperty;
        set
        {
            _userProperty = value;
            OnPropertyChanged(nameof(UserProperty));
        }
    }

    public LoginViewModel()
    {
    }
    public LoginViewModel(UserRepository userRepository, INavigationService navigationService)
    {
        _userService = new UserService(userRepository);
        _navigationService = navigationService;
        LoginCommand = new AutoRefreshCommand(ExecuteCommand, CanExecuteCommand, this);
        _userProperty = new UserProperty();

        _userProperty.PropertyChanged += (s, e) => ((AutoRefreshCommand)LoginCommand).RaiseCanExecuteChanged();
        GoToRegisterCommand = new Command(async () => await OnGoToRegisterClicked());

    }

    protected override async Task ExecuteCommand()
    {
        UXAnimation.IsLoading = true;
        try
        {
            await Task.Delay(2000);
            await _userService.LoginUser(UserProperty.Email, UserProperty.Password);
            AlertMessenger.SendMessage($"User {UserProperty.Email} logged in successfully", true);
            await _navigationService.NavigateToAsync<HomePage>();
        }
        catch (Exception ex)
        {
            AlertMessenger.SendMessage(ex.Message, false);
        }
        finally
        {
            UXAnimation.IsLoading = false;
        }
    }

    private async Task OnGoToRegisterClicked()
    {
        await _navigationService.NavigateToAsync<RegisterPage>();
    }

    protected override bool CanExecuteCommand()
    {
        return !UXAnimation.IsLoading &&
               !string.IsNullOrWhiteSpace(UserProperty.Email) &&
               !string.IsNullOrWhiteSpace(UserProperty.Password);
        //    _userService.ValidateEmail(_user.Email) &&
        //    _userService.ValidatePassword(_user.Password);
    }
}
