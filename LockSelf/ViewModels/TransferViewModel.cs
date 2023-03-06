using Core.Models;
using Core.Services;
using Core.Services.Impl;
using LockSelf.ViewModels.Commands;
using Microsoft.Office.Interop.Outlook;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Messages;
using ToastNotifications.Position;
using File = Core.Models.File;

namespace LockSelf.ViewModels
{
    public class TransferViewModel : BaseViewModel
    {
        #region Auth     
        private IAuthService _authService;
        private IAuthSSOService _authSSOService;

        private User _user;

        public string Email
        {
            get => _user.userSession.userEmail;
            set
            {
                _user.userSession.userEmail = value;
                RaisePropertyChanged("Email");
            }
        }

        public string Password
        {
            get => _user.userSession.Password;
            set
            {
                _user.userSession.Password = value;
                RaisePropertyChanged("Password");

            }
        }

        //public string UserType
        //{
        //    get => _user.userSession.organizationNames.userType;
        //    set
        //    {
        //        _user.userSession.organizationNames.userType = value;
        //        RaisePropertyChanged("UserType");
        //    }
        //}

        private string _txtBtnLoginSSO;
        public string TxtBtnLoginSSO
        {
            get => _txtBtnLoginSSO;
            set
            {
                _txtBtnLoginSSO = value;
                RaisePropertyChanged("TxtBtnLoginSSO");

            }
        }

        private string _hiddenText;
        public string HiddenText
        {
            get => _hiddenText;
            set
            {
                _hiddenText = value;
                RaisePropertyChanged("HiddenText");

            }
        }

        public string Access_token
        {
            get => _user.token;
            set => _user.token = value;
        }


        private bool _loginPage;
        public bool LoginPage
        {
            get { return _loginPage; }
            set
            {
                _loginPage = value;
                RaisePropertyChanged("LoginPage");
            }
        }

        private bool _loginDone;
        public bool LoginDone
        {
            get { return _loginDone; }
            set
            {
                _loginDone = value;
                RaisePropertyChanged("LoginDone");
            }
        }

        private string _labelPopUp;
        public string LabelPopUp
        {
            get { return _labelPopUp; }
            set
            {
                _labelPopUp = value;
                RaisePropertyChanged("LabelPopUp");
            }
        }

        private string _txtPopUp;
        public string TxtPopUp
        {
            get { return _txtPopUp; }
            set
            {
                _txtPopUp = value;
                RaisePropertyChanged("TxtPopUp");
            }
        }

        private bool _showPopUP;
        public bool ShowPopUp
        {
            get { return _showPopUP; }
            set
            {
                _showPopUP = value;
                RaisePropertyChanged("ShowPopUp");
            }
        }

        public ICommand LoginCommand { get; set; }
        public ICommand LoginSSOCommand { get; set; }
        public ICommand LogOutCommand { get; set; }

        public void Login(string password)
        {
            try
            {
                string urlApi = _authSSOService.GetUrlApi(Email);
                var user = _authService.AuthenticateUser(urlApi, Email, password);
                if (user.userSession != null)
                {
                    Username = user.userSession.username;
                    LockSelf.Utils.Globals.LoggedInUser = user;
                    _loginDone = true;
                }
                else {
                    _loginDone = false;
                    LabelPopUp = "Erreur";
                    TxtPopUp = "Email et/ou mot de passe incorrect(s), veuillez réessayer, svp";
                    ShowPopUp = true;
                }
            }
            catch (System.Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        public async Task LoginSSO()
        {
            TxtBtnLoginSSO = "En cours";
            try
            {
                string urlApi = _authSSOService.GetUrlApi(Email);

                Debug.WriteLine(urlApi);

                var urlBrowser = _authSSOService.FirstStep(urlApi);
                Uri uriResult;
                bool isUrl = Uri.TryCreate(urlBrowser, UriKind.Absolute, out uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

                if (!isUrl)
                {
                    //MessageBox.Show("Vous n'êtes pas eligible au SSO !");
                    LabelPopUp = "Erreur";
                    TxtPopUp = "Vous n'êtes pas eligible au SSO !";
                    ShowPopUp = true;
                    TxtBtnLoginSSO = "Se Connecter";
                    return;
                }

                Process.Start(urlBrowser);

                var user = await _authSSOService.VerifConnect(urlApi);

                if (user == null) {       
                    //MessageBox.Show("Temps écoulé ! Veuillez cliquer de nouveau sur SE CONNECTER, svp");
                    LabelPopUp = "Erreur";
                    TxtPopUp = "Temps écoulé ! Veuillez cliquer de nouveau sur SE CONNECTER, svp";
                    ShowPopUp = true;
                    TxtBtnLoginSSO = "Se Connecter";
                    return;
                }

                TxtBtnLoginSSO = "Se Connecter";

                if (user.userSession != null)
                {
                    Username = user.userSession.username;
                    LockSelf.Utils.Globals.LoggedInUser = user;
                    _loginDone = true;
                }
                else
                {
                    _loginDone = false;
                    LabelPopUp = "Erreur";
                    TxtPopUp = "Email et/ou mot de passe incorrect(s), veuillez réessayer, svp";
                    ShowPopUp = true;
                }
            }
            catch (System.Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        public void LogOut()
        {
            try
            {
                string urlApi = _authSSOService.GetUrlApi(Email);
                _authService.Disconnect(urlApi);
                LockSelf.Utils.Globals.LoggedInUser = null;
                
            }
            catch (System.Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }
        
        #endregion

        #region Transfer

        private ITransferService _transferService;

        private string _url;
        public string Url
        {
            get => _url;
            set
            {
                _url = value;
                RaisePropertyChanged("Url");
            }
        }

        private string _username;
        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                RaisePropertyChanged("Username");
            }
        }

        private Transfer _transfer;

        public DateTime ExpirationDate
        {
            get => _transfer.expirationDate;
            set
            {
                _transfer.expirationDate = value;
                RaisePropertyChanged("ExpirationDate");
            }
        }

        public Nullable<DateTime> DisplayDateStart { get; set; }

        public string PasswordTransfer
        {
            get => _transfer.password;
            set
            {
                _transfer.password = value;
                RaisePropertyChanged("PasswordTransfer");

            }
        }
        private bool _showPassword;
        public bool ShowPassword
        {
            get => _showPassword;
            set
            {
                if (_showPassword != value)
                {
                    _showPassword = value;
                    RaisePropertyChanged("ShowPassword");
                }
            }
        }


        public string PrefixPhone
        {
            get => _transfer.prefixPhone;
            set
            {
                _transfer.prefixPhone = value;
                RaisePropertyChanged("PrefixPhone");

            }
        }

        public string Phone
        {
            get => _transfer.phone;
            set
            {
                _transfer.phone = value;
                RaisePropertyChanged("Phone");

            }
        }

        public int UploadNumber
        {
            get => _transfer.uploadNumber;
            set
            {
                _transfer.uploadNumber = value;
                RaisePropertyChanged("UploadNumber");

            }
        }

        private List<int> _property;
        public List<int> Property
        {
            get
            {
                return new List<int>() { 1 , 2, 3 };
            }
            set
            {
                _property = value;
            }
        }
        

        public ObservableCollection<FileViewModel> Files { get; } = new ObservableCollection<FileViewModel>();

        private FileViewModel _fileSelect;
        public FileViewModel FileSelect
        {
            get
            {
                return _fileSelect;
            }
            set
            {
                _fileSelect = value;
                RaisePropertyChanged("FileSelect");
            }
        }

        private double _maxTransfer;
        public double MaxTransfer
        {
            get { return _maxTransfer; }
            set
            {
                _maxTransfer = value;
                RaisePropertyChanged("MaxTransfer");
            }
        }

        public string MaxTransfer2
        {
            get { return String.Format("{0:0.00} Mo restant", _maxTransfer / 1024 / 1024); }
            set
            {
                _maxTransfer = Convert.ToDouble(value);
                RaisePropertyChanged("MaxTransfer2");
            }
        }

        private string _count;
        public string Count
        {
            get 
            {
                if (int.Parse(_count) > 1)
                {
                    return _count + " fichiers";
                }
                else {
                    return _count + " fichier";
                }
            }
            set
            {
                _count = value;
                RaisePropertyChanged("Count");
            }
        }

        private bool _jobDone;
        public bool JobDone
        {
            get { return _jobDone; }
            set
            {
                _jobDone = value;
                RaisePropertyChanged("JobDone");
            }
        }

        private bool _progressLoading;
        public bool ProgressLoading
        {
            get { return _progressLoading; }
            set
            {
                _progressLoading = value;
                RaisePropertyChanged("ProgressLoading");
            }
        }

        private double _currentProgress;
        public double CurrentProgress
        {
            get { return _currentProgress; }
            private set
            {
                _currentProgress = value;
                RaisePropertyChanged("CurrentProgress");
            }
        }

        private bool _jobSuccess;
        public bool JobSuccess
        {
            get { return _jobSuccess; }
            set
            {
                _jobSuccess = value;
                RaisePropertyChanged("JobSuccess");
            }
        }

        private string _iso2;
        public string Iso2
        {
            get => _iso2;
            set
            {
                _iso2 = value;
                RaisePropertyChanged("Iso2");
            }
        }

        public ICommand AddCommand { get; set; }
        public ICommand DeleteAllCommand { get; set; }
        public ICommand DeleteOneCommand { get; set; }
        public ICommand GenPasswordCommand { get; set; }
        public ICommand GenLinkCommand { get; set; }
        public ICommand Add2Command { get; set; }

        public void Add()
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                //To where your opendialog box get starting location. My initial directory location is desktop.
                ofd.InitialDirectory = "C://Desktop";
                //Your opendialog box title name.
                ofd.Title = "Select file to be upload.";
                //which type file format you want to upload in database. just add them.
                ofd.Filter = "Select Valid Document(*.pdf; *.doc; *.xlsx; *.html; *.jpg; *.png; *.mp4; *.ppt; *.txt)|*.pdf; *.doc; *.xlsx; *.html; *.jpg; *.png; *.mp4; *.ppt; *.txt;";
                //FilterIndex property represents the index of the filter currently selected in the file dialog box.
                ofd.FilterIndex = 1;
                // Allow the user to select multiple images.
                ofd.Multiselect = true;
                try
                {
                    if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        if (ofd.CheckFileExists)
                        {
                            AddFile(ofd.FileNames);                       
                        }
                    }
                    else
                    {
                        System.Windows.MessageBox.Show("Please Upload document.");
                        LabelPopUp = "Erreur";
                        TxtPopUp = "Une erreur est survenue, veuillez réessayer, svp";
                        ShowPopUp = true;
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
        }

        public void Clear()
        {
            Files.Clear();
            Count = Files.Count.ToString();
            MaxTransfer = 104857600;
            var c = MaxTransfer;
            MaxTransfer2 = c.ToString();
        }

        public void DeleteOne()
        {
            var c = MaxTransfer + FileSelect.File.KbWeight;
            MaxTransfer -= FileSelect.File.KbWeight;
            MaxTransfer2 = c.ToString();
            Files.RemoveAt(Files.IndexOf(FileSelect));
            Count = Files.Count.ToString();
        }
     
        public void GeneratePassword()
        {
            PasswordTransfer = _transferService.CreatePassword(8);
        }

        public async void GenerateLink()
        {
            try {
                Inspector inspector = Globals.ThisAddIn.Application.ActiveInspector();
                MailItem mailItem = inspector.CurrentItem as MailItem;
                var user = LockSelf.Utils.Globals.LoggedInUser;

                List<File> files = Files.Select(vm => vm.File).ToList();
                Debug.WriteLine("UploadNumber" + UploadNumber.ToString());

                string s = string.Format("{0:yyyyMMdd}", DisplayDateStart);
                Debug.WriteLine("Date" +  s);

                Debug.WriteLine("PrefixPhone -> " + (string)PrefixPhone);

                var fullPhone = PrefixPhone + " " + Phone;
                Debug.WriteLine("fullPhone -> " + fullPhone);

                string urlApi = _authService.GetUrlApi(Email);

                var progress = new Progress<int>(x => CurrentProgress = x);

                ProgressLoading = true;
                JobSuccess = await Task.Run(() =>
                {
                    var link = _transferService.UploadFileAsync(urlApi, user, UploadNumber, files, progress, s, PasswordTransfer, Phone);
                    
                    if (link != null)
                    {
                        var r = JsonConvert.DeserializeObject<TransferResponse>(link.Result);
                        Debug.WriteLine("response -> " + r.downloadLink);

                        mailItem.BodyFormat = OlBodyFormat.olFormatHTML;
                        //string l = "<span style=\"background-color: blue;height:100px;width:100px;text-align:center\"> test </span>";
                        //string l = "<div style=\"overflow: hidden;border: 1px solid #000;width: 200px;position: relative;padding: 30px;background-color: blue;\"><style=\"overflow: hidden;border: 1px solid #000;width: 20px;padding: 10px;background-color: orange;\">submit</div></div>";

                        //mailItem.HTMLBody = l + mailItem.Body;

                        // Renvoie le link de telechargement ds le body
                        mailItem.HTMLBody += "<html>";
                        mailItem.HTMLBody += "<body>";
                        mailItem.HTMLBody += "<div style=\"background-color: orange;\">";

                        mailItem.HTMLBody += "<div style=\"overflow: hidden;border: 0px solid transparent;background-color: orange;padding:80px;\">";

                        mailItem.HTMLBody += "<a style=\"color: white;text-decoration: none;\" href=" + r.downloadLink + ">Download</a>";
                        mailItem.HTMLBody += "</div>";

                        mailItem.HTMLBody += "</div>";
                        mailItem.HTMLBody += "</body>";
                        mailItem.HTMLBody += "</html>";
                    }
                    else
                    {
                        LabelPopUp = "Erreur";
                        TxtPopUp = "Une erreur est survenue, veuillez réessayer, svp";
                        ShowPopUp = true;
                        ProgressLoading = false;
                    }
                    return true;
                })
                .ConfigureAwait(false);

                Files.Clear();
                PasswordTransfer = "";
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public void AddFile(string[] files)
        {
            foreach (string file in files) {
                FileInfo fi = new FileInfo(file);
                File f = null;
                string contentType = "";
                switch (System.IO.Path.GetExtension(file).ToLower())
                {
                    case ".jpg":
                        contentType = "image/jpeg";
                        f = new File(file, fi.Length, "JPEG", "/LockSelf;component/Resources/jpeg.png");
                        break;
                    case ".png":
                        contentType = "image/png";
                        f = new File(file, fi.Length, "PNG", "/LockSelf;component/Resources/png.png");                      
                        break;
                    case ".docx":
                        contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                        f = new File(file, fi.Length, "DOCX", "/LockSelf;component/Resources/docx-file.png");                    
                        break;
                    case ".xlsx":
                        contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        f = new File(file, fi.Length, "XLSX", "/LockSelf;component/Resources/xlsx.png");                      
                        break;
                    case ".pdf":
                        contentType = "application/pdf";
                        f = new File(file, fi.Length, "PDF", "/LockSelf;component/Resources/pdf.png");                     
                        break;
                    case ".txt":
                        contentType = "application/vnd.ms-powerpoint";
                        f = new File(file, fi.Length, "TXT", "/LockSelf;component/Resources/txt.png");                   
                        break;
                    case ".ppt":
                        contentType = "application/vnd.openxmlformats-officedocument.presentationml.presentation";
                        f = new File(file, fi.Length, "PPT", "/LockSelf;component/Resources/ppt.png");                   
                        break;
                    case ".html":
                        contentType = "application/vnd.openxmlformats-officedocument.presentationml.presentation";
                        f = new File(file, fi.Length, "HTML", "/LockSelf;component/Resources/html.png");
                        break;
                }
                if (f.KbWeight > MaxTransfer)
                {
                    Count = Files.Count.ToString();
                }
                else
                {
                    if (!Files.Any(p => p.File.Title == file))
                    {
                        Files.Add(new FileViewModel(f));
                        Count = Files.Count.ToString();
                        var c = MaxTransfer - fi.Length;
                        MaxTransfer -= fi.Length;
                        MaxTransfer2 = c.ToString();
                    }
                    else
                    {
                        LabelPopUp = "Erreur";
                        TxtPopUp = "Le fichier " + file + " existe déjà !";
                        ShowPopUp = true;
                    }
                }
            }
        }

        #endregion

        #region Constructors

        public TransferViewModel()
        {
        }

        public TransferViewModel(ITransferService transferService, IAuthService authService, IAuthSSOService authSSOService)
        {
            _transferService = transferService;
            _authService = authService;
            _authSSOService = authSSOService;
            _transfer = new Transfer();
            _maxTransfer = 104857600;
            AddCommand = new RelayCommand(p => Add());
            DeleteAllCommand = new RelayCommand(p => Clear());
            DeleteOneCommand = new RelayCommand(p => DeleteOne());
            GenPasswordCommand = new RelayCommand(p => GeneratePassword());
            GenLinkCommand = new RelayCommand(p =>  GenerateLink());
            //Files.Add(new FileViewModel(new File("", 0L)));
            Add2Command = new RelayCommand(p => AddFile((string[])p));

            //UserSession us = new UserSession("", "Test007!");
            //_user = new User() { userSession = us };

            UserSession us = new UserSession("m.rezgui06@gmail.com", "");
            TxtBtnLoginSSO = "Se Connecter";
            //UserSession us = new UserSession("", "");

            _user = new User() { userSession = us };
            Url = "https://api.lockself.com/api";
            LoginCommand = new RelayCommand(p => Login((string)p));
            LoginSSOCommand = new AsyncRelayCommand(async p => await LoginSSO());

            LogOutCommand = new RelayCommand(p => LogOut());

            ShowPassword = true;
            ShowPopUp = false;


        }
        #endregion

        private void addToBox() { 
        
        
        }

    }
}
