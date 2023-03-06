using System;
using System.Security;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace LockSelf.Controls
{
    internal class CustomPasswordBox : TextBox
    {
        #region Member Variables
        /// <summary>
        ///   Dependency property to hold watermark for CustomPasswordBox
        /// </summary>
        public static readonly DependencyProperty PasswordProperty =
          DependencyProperty.Register(
            "Password", typeof(SecureString), typeof(CustomPasswordBox), new UIPropertyMetadata(new SecureString()));

        public static readonly DependencyProperty HiddenTextProperty =
          DependencyProperty.Register(
            "HiddenText", typeof(string), typeof(CustomPasswordBox), new UIPropertyMetadata(string.Empty));

        /// <summary>
        ///   Private member holding mask visibile timer
        /// </summary>
        private readonly DispatcherTimer _maskTimer;
        #endregion

        #region Constructors
        /// <summary>
        ///   Initialises a new instance of the LifeStuffPasswordBox class.
        /// </summary>
        public CustomPasswordBox()
        {
            PreviewTextInput += OnPreviewTextInput;
            PreviewKeyDown += OnPreviewKeyDown;
            CommandManager.AddPreviewExecutedHandler(this, PreviewExecutedHandler);
            _maskTimer = new DispatcherTimer { Interval = new TimeSpan(0, 0, 0, 1) };
            _maskTimer.Tick += (sender, args) => MaskAllDisplayText();
        }
        #endregion

        #region Commands & Properties
        /// <summary>
        ///   Gets or sets dependency Property implementation for Password
        /// </summary>
        public SecureString Password
        {
            get
            {
                return (SecureString)GetValue(PasswordProperty);
            }

            set
            {
                SetValue(PasswordProperty, value);
            }
        }

        public string HiddenText
        {
            get
            {
                return (string)GetValue(HiddenTextProperty);
            }

            set
            {
                SetValue(HiddenTextProperty, value);
            }
        }
        #endregion

        #region Methods
        /// <summary>
        ///   Method to handle PreviewExecutedHandler events
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="executedRoutedEventArgs">Event Text Arguments</param>
        private static void PreviewExecutedHandler(object sender, ExecutedRoutedEventArgs executedRoutedEventArgs)
        {
            if (executedRoutedEventArgs.Command == ApplicationCommands.Copy ||
                executedRoutedEventArgs.Command == ApplicationCommands.Cut ||
                executedRoutedEventArgs.Command == ApplicationCommands.Paste)
            {
                executedRoutedEventArgs.Handled = true;
            }
        }

        /// <summary>
        ///   Method to handle PreviewTextInput events
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="textCompositionEventArgs">Event Text Arguments</param>
        private void OnPreviewTextInput(object sender, TextCompositionEventArgs textCompositionEventArgs)
        {
            AddToSecureString(textCompositionEventArgs.Text);
            textCompositionEventArgs.Handled = true;
        }

        /// <summary>
        ///   Method to handle PreviewKeyDown events
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="keyEventArgs">Event Text Arguments</param>
        private void OnPreviewKeyDown(object sender, KeyEventArgs keyEventArgs)
        {
            Key pressedKey = keyEventArgs.Key == Key.System ? keyEventArgs.SystemKey : keyEventArgs.Key;
            switch (pressedKey)
            {
                case Key.Space:
                    AddToSecureString(" ");
                    keyEventArgs.Handled = true;
                    break;
                case Key.Back:
                case Key.Delete:
                    if (SelectionLength > 0)
                    {
                        RemoveFromSecureString(SelectionStart, SelectionLength);
                    }
                    else if (pressedKey == Key.Delete && CaretIndex < Text.Length)
                    {
                        RemoveFromSecureString(CaretIndex, 1);
                    }
                    else if (pressedKey == Key.Back && CaretIndex > 0)
                    {
                        int caretIndex = CaretIndex;
                        if (CaretIndex > 0 && CaretIndex < Text.Length)
                            caretIndex = caretIndex - 1;
                        RemoveFromSecureString(CaretIndex - 1, 1);
                        CaretIndex = caretIndex;
                    }

                    keyEventArgs.Handled = true;
                    break;
            }
        }

        /// <summary>
        ///   Method to add new text into SecureString and process visual output
        /// </summary>
        /// <param name="text">Text to be added</param>
        private void AddToSecureString(string text)
        {
            if (SelectionLength > 0)
            {
                RemoveFromSecureString(SelectionStart, SelectionLength);
            }

            foreach (char c in text)
            {
                int caretIndex = CaretIndex;
                Password.InsertAt(caretIndex, c);
                HiddenText = HiddenText.Insert(caretIndex, c.ToString());
                MaskAllDisplayText();
                if (caretIndex == Text.Length)
                {
                    _maskTimer.Stop();
                    _maskTimer.Start();
                    Text = Text.Insert(caretIndex++, c.ToString());
                }
                else
                {
                    Text = Text.Insert(caretIndex++, "*");
                }
                CaretIndex = caretIndex;
            }
        }

        /// <summary>
        ///   Method to remove text from SecureString and process visual output
        /// </summary>
        /// <param name="startIndex">Start Position for Remove</param>
        /// <param name="trimLength">Length of Text to be removed</param>
        private void RemoveFromSecureString(int startIndex, int trimLength)
        {
            int caretIndex = CaretIndex;
            for (int i = 0; i < trimLength; ++i)
            {
                Password.RemoveAt(startIndex);
                HiddenText = HiddenText.Remove(startIndex, 1);
            }

            Text = Text.Remove(startIndex, trimLength);
            CaretIndex = caretIndex;
        }

        private void MaskAllDisplayText()
        {
            _maskTimer.Stop();
            int caretIndex = CaretIndex;
            Text = new string('*', Text.Length);
            CaretIndex = caretIndex;
        }
        #endregion
    }
}
