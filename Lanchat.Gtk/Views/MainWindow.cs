using System;
using System.Linq;
using System.Net;
using Gtk;
using Key = Gdk.Key;
using UI = Gtk.Builder.ObjectAttribute;
using WrapMode = Pango.WrapMode;

namespace Lanchat.Gtk.Views
{
    public class MainWindow : Window
    {
#pragma warning disable 649
        // Main content
        [UI] private ScrolledWindow scroll;
        [UI] private ListBox chat;
        [UI] private Entry input;

        // Settings menu
        [UI] private Popover menu;
        [UI] private ToggleButton menuToggle;
        [UI] private Entry menuNicknameField;
        [UI] private Button menuSaveButton;

        // Connect menu
        [UI] private Popover connectMenu;
        [UI] private ToggleButton connectMenuToggle;
        [UI] private Entry connectIpAddress;
        [UI] private Entry connectPortNumber;
        [UI] private Button connectButton;

        // Sidebar
        [UI] private ListBox connectedList;
        [UI] private ListBox onlineList;
#pragma warning restore 649

        private string lastMessageAuthor;

        public MainWindow() : this(new Builder("MainWindow.glade"))
        {
        }

        private MainWindow(Builder builder) : base(builder.GetObject("MainWindow").Handle)
        {
            builder.Autoconnect(this);

            DeleteEvent += Window_DeleteEvent;
            input.KeyReleaseEvent += InputOnKeyReleaseEvent;
            menu.Closed += MenuOnClosed;
            menuToggle.Toggled += MenuToggleOnToggled;
            menuSaveButton.Clicked += MenuSaveButtonOnClicked;
            connectMenu.Closed += ConnectMenuOnClosed;
            connectMenuToggle.Toggled += ConnectMenuToggleOnToggled;
            connectButton.Clicked += ConnectButtonOnClicked;

            menuNicknameField.Text = Program.Config.Nickname;
            connectPortNumber.Text = Program.Config.Port.ToString();
            Program.Network.ConnectionCreated += (sender, node) => { _ = new NodeEventsHandlers(node, this); };
        }

        // UI Events
        private void MenuToggleOnToggled(object sender, EventArgs e)
        {
            if (menuToggle.Active) menu.ShowAll();
        }

        private void MenuOnClosed(object sender, EventArgs e)
        {
            menuToggle.Active = false;
        }

        private void MenuSaveButtonOnClicked(object sender, EventArgs e)
        {
            Program.Config.Nickname = menuNicknameField.Text;
        }

        private void ConnectMenuOnClosed(object sender, EventArgs e)
        {
            connectMenuToggle.Active = false;
        }

        private void ConnectMenuToggleOnToggled(object sender, EventArgs e)
        {
            if (connectMenuToggle.Active) connectMenu.ShowAll();
        }

        private void ConnectButtonOnClicked(object sender, EventArgs e)
        {
            if (IPAddress.TryParse(connectIpAddress.Text, out var ipAddress))
            {
                Program.Network.Connect(ipAddress, int.Parse(connectPortNumber.Text));
                connectMenu.Hide();
            }
        }

        private void InputOnKeyReleaseEvent(object o, KeyReleaseEventArgs args)
        {
            if (args.Event.Key != Key.Return) return;
            AddChatEntry($"{Program.Config.Nickname}#0000", input.Text);
            Program.Network.BroadcastMessage(input.Text);
            input.Text = string.Empty;
        }

        private static void Window_DeleteEvent(object sender, DeleteEventArgs a)
        {
            Application.Quit();
        }

        public void AddChatEntry(string nickname, string message)
        {
            var box = new Box(Orientation.Vertical, 0);

            var sender = new Label
            {
                Valign = Align.Start,
                Halign = Align.Start,
                Markup = $"<b>{nickname}</b>"
            };

            var content = new Label(message)
            {
                Valign = Align.Start,
                Halign = Align.Start,
                Wrap = true,
                LineWrapMode = WrapMode.Char,
                Selectable = true
            };

            if (lastMessageAuthor != nickname)
            {
                box.Add(sender);
                lastMessageAuthor = nickname;
            }

            box.Add(content);
            chat.Add(new ListBoxRow {Child = box});
            chat.ShowAll();
            scroll.Vadjustment.Value = scroll.Vadjustment.Upper;
        }

        public void AddConnected(string nickname, Guid id)
        {
            connectedList.Add(new ListBoxRow
            {
                Child = new Label(nickname)
                {
                    Margin = 2
                },
                Halign = Align.Start,
                Name = $"{id}-cl"
            });
            connectedList.ShowAll();
        }

        public void RemoveConnected(Guid id)
        {
            connectedList.Remove(connectedList.Children.FirstOrDefault(x => x.Name == $"{id}-cl"));
        }
    }
}