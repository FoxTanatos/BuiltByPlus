using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace BuiltBy
{
    public sealed class BuiltByControl : UserControl
    {
        private readonly Plugin _plugin;
        private readonly ComboBox _languageBox = new ComboBox();
        private readonly TabControl _tabs = new TabControl();
        private readonly ListBox _tagList = new ListBox();
        private readonly TextBox _tagInput = new TextBox();
        private readonly ListBox _identityNameList = new ListBox();
        private readonly TextBox _identityNameInput = new TextBox();
        private readonly CheckBox _autoCleanupCheckBox = new CheckBox();
        private readonly CheckBox _safeModeCheckBox = new CheckBox();
        private readonly TextBlock _cleanupStatus = new TextBlock();

        private StackPanel _root;
        private TextBlock _languageLabel;
        private TextBlock _overviewTitle;
        private TextBlock _overviewText;
        private TextBlock _overviewSafety;
        private TextBlock _factionTitle;
        private TextBlock _factionText;
        private TextBlock _restartNote;
        private TextBlock _astronautTitle;
        private TextBlock _astronautText;
        private Button _addTagButton;
        private Button _removeTagButton;
        private Button _saveButton;
        private Button _addNameButton;
        private Button _removeNameButton;
        private Button _scanButton;
        private Button _deleteButton;
        private TabItem _overviewTab;
        private TabItem _factionTab;
        private TabItem _astronautTab;

        public BuiltByControl(Plugin plugin)
        {
            _plugin = plugin;
            BuildInterface();
            LoadValues();
            ApplyLanguage();
        }

        private void BuildInterface()
        {
            ScrollViewer scroll = new ScrollViewer
            {
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto
            };

            _root = new StackPanel
            {
                Margin = new Thickness(12)
            };
            scroll.Content = _root;

            _root.Children.Add(BuildLanguageRow());

            _overviewTab = new TabItem { Content = BuildOverviewTab() };
            _factionTab = new TabItem { Content = BuildFactionTab() };
            _astronautTab = new TabItem { Content = BuildAstronautTab() };

            _tabs.Items.Add(_overviewTab);
            _tabs.Items.Add(_factionTab);
            _tabs.Items.Add(_astronautTab);
            _tabs.SelectedIndex = 0;
            _root.Children.Add(_tabs);

            Content = scroll;
        }

        private Grid BuildLanguageRow()
        {
            Grid row = new Grid { Margin = new Thickness(0, 0, 0, 10) };
            row.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            row.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            _languageLabel = new TextBlock
            {
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 8, 0)
            };
            Grid.SetColumn(_languageLabel, 0);
            row.Children.Add(_languageLabel);

            _languageBox.MinWidth = 130;
            _languageBox.Items.Add(new LanguageItem(BuiltByText.Ukrainian, BuiltByText.Get(BuiltByText.Ukrainian, "Ukrainian")));
            _languageBox.Items.Add(new LanguageItem(BuiltByText.English, BuiltByText.Get(BuiltByText.English, "English")));
            _languageBox.SelectionChanged += LanguageChanged;
            Grid.SetColumn(_languageBox, 1);
            row.Children.Add(_languageBox);

            return row;
        }

        private StackPanel BuildOverviewTab()
        {
            StackPanel panel = new StackPanel { Margin = new Thickness(8) };
            _overviewTitle = Header(string.Empty);
            _overviewText = Text(string.Empty);
            _overviewSafety = Text(string.Empty, true);
            panel.Children.Add(_overviewTitle);
            panel.Children.Add(_overviewText);
            panel.Children.Add(_overviewSafety);
            return panel;
        }

        private StackPanel BuildFactionTab()
        {
            StackPanel panel = new StackPanel { Margin = new Thickness(8) };
            _factionTitle = Header(string.Empty);
            _factionText = Text(string.Empty);
            panel.Children.Add(_factionTitle);
            panel.Children.Add(_factionText);

            _tagList.Height = 170;
            _tagList.Margin = new Thickness(0, 0, 0, 8);
            panel.Children.Add(_tagList);

            Grid row = BuildInputRow(_tagInput, out _addTagButton, out _removeTagButton, AddTag, RemoveTag);
            panel.Children.Add(row);

            _saveButton = new Button
            {
                MinWidth = 150,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(0, 0, 0, 8)
            };
            _saveButton.Click += SaveChanges;
            panel.Children.Add(_saveButton);

            _restartNote = Text(string.Empty, true);
            panel.Children.Add(_restartNote);
            return panel;
        }

        private StackPanel BuildAstronautTab()
        {
            StackPanel panel = new StackPanel { Margin = new Thickness(8) };
            _astronautTitle = Header(string.Empty);
            _astronautText = Text(string.Empty);
            panel.Children.Add(_astronautTitle);
            panel.Children.Add(_astronautText);

            _identityNameList.Height = 140;
            _identityNameList.Margin = new Thickness(0, 0, 0, 8);
            panel.Children.Add(_identityNameList);

            Grid row = BuildInputRow(_identityNameInput, out _addNameButton, out _removeNameButton, AddIdentityName, RemoveIdentityName);
            panel.Children.Add(row);

            _autoCleanupCheckBox.Margin = new Thickness(0, 4, 0, 8);
            panel.Children.Add(_autoCleanupCheckBox);

            _safeModeCheckBox.Margin = new Thickness(0, 0, 0, 8);
            panel.Children.Add(_safeModeCheckBox);

            StackPanel cleanupButtons = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0, 0, 0, 8)
            };

            _scanButton = new Button
            {
                MinWidth = 120,
                Margin = new Thickness(0, 0, 8, 0)
            };
            _scanButton.Click += ScanAstronautIdentities;
            cleanupButtons.Children.Add(_scanButton);

            _deleteButton = new Button
            {
                MinWidth = 140
            };
            _deleteButton.Click += DeleteAstronautIdentities;
            cleanupButtons.Children.Add(_deleteButton);

            panel.Children.Add(cleanupButtons);
            _cleanupStatus.TextWrapping = TextWrapping.Wrap;
            panel.Children.Add(_cleanupStatus);
            return panel;
        }

        private static TextBlock Header(string text)
        {
            return new TextBlock
            {
                Text = text,
                FontSize = 16,
                FontWeight = FontWeights.SemiBold,
                Margin = new Thickness(0, 0, 0, 8)
            };
        }

        private static TextBlock Text(string text, bool bold = false)
        {
            return new TextBlock
            {
                Text = text,
                TextWrapping = TextWrapping.Wrap,
                FontWeight = bold ? FontWeights.SemiBold : FontWeights.Normal,
                Margin = new Thickness(0, 0, 0, 8)
            };
        }

        private static Grid BuildInputRow(TextBox input, out Button addButton, out Button removeButton, RoutedEventHandler addHandler, RoutedEventHandler removeHandler)
        {
            Grid inputRow = new Grid { Margin = new Thickness(0, 0, 0, 8) };
            inputRow.ColumnDefinitions.Add(new ColumnDefinition());
            inputRow.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            inputRow.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            input.MinWidth = 140;
            input.Margin = new Thickness(0, 0, 8, 0);
            Grid.SetColumn(input, 0);
            inputRow.Children.Add(input);

            addButton = new Button
            {
                MinWidth = 80,
                Margin = new Thickness(0, 0, 8, 0)
            };
            addButton.Click += addHandler;
            Grid.SetColumn(addButton, 1);
            inputRow.Children.Add(addButton);

            removeButton = new Button
            {
                MinWidth = 80
            };
            removeButton.Click += removeHandler;
            Grid.SetColumn(removeButton, 2);
            inputRow.Children.Add(removeButton);

            return inputRow;
        }

        private void LoadValues()
        {
            string language = _plugin.GetLanguage();
            foreach (LanguageItem item in _languageBox.Items)
            {
                if (item.Code == language)
                {
                    _languageBox.SelectedItem = item;
                    break;
                }
            }

            _tagList.Items.Clear();
            foreach (string tag in _plugin.GetConfiguredFactionTags().OrderBy(x => x))
                _tagList.Items.Add(tag);

            AstronautCleanupConfig cleanup = _plugin.GetAstronautCleanupConfig();
            _identityNameList.Items.Clear();
            foreach (string name in cleanup.DisplayNames.OrderBy(x => x))
                _identityNameList.Items.Add(name);

            _autoCleanupCheckBox.IsChecked = cleanup.AutoCleanupEnabled;
            _safeModeCheckBox.IsChecked = cleanup.SafeModeEnabled;
            _cleanupStatus.Text = string.Empty;
        }

        private void ApplyLanguage()
        {
            string language = GetSelectedLanguage();
            _languageLabel.Text = BuiltByText.Get(language, "Language") + ":";

            _overviewTab.Header = BuiltByText.Get(language, "TabOverview");
            _factionTab.Header = BuiltByText.Get(language, "TabFactions");
            _astronautTab.Header = BuiltByText.Get(language, "TabAstronauts");

            _overviewTitle.Text = BuiltByText.Get(language, "OverviewTitle");
            _overviewText.Text = BuiltByText.Get(language, "OverviewText");
            _overviewSafety.Text = BuiltByText.Get(language, "OverviewSafety");

            _factionTitle.Text = BuiltByText.Get(language, "FactionTitle");
            _factionText.Text = BuiltByText.Get(language, "FactionText");
            _restartNote.Text = BuiltByText.Get(language, "RestartNote");
            _addTagButton.Content = BuiltByText.Get(language, "Add");
            _removeTagButton.Content = BuiltByText.Get(language, "Remove");
            _saveButton.Content = BuiltByText.Get(language, "Save");

            _astronautTitle.Text = BuiltByText.Get(language, "AstronautTitle");
            _astronautText.Text = BuiltByText.Get(language, "AstronautText");
            _addNameButton.Content = BuiltByText.Get(language, "Add");
            _removeNameButton.Content = BuiltByText.Get(language, "Remove");
            _autoCleanupCheckBox.Content = BuiltByText.Get(language, "AutoCleanup");
            _safeModeCheckBox.Content = BuiltByText.Get(language, "SafeMode");
            _scanButton.Content = BuiltByText.Get(language, "FindMatches");
            _deleteButton.Content = BuiltByText.Get(language, "DeleteFound");
        }

        private string GetSelectedLanguage()
        {
            LanguageItem item = _languageBox.SelectedItem as LanguageItem;
            return item == null ? _plugin.GetLanguage() : item.Code;
        }

        private void LanguageChanged(object sender, SelectionChangedEventArgs args)
        {
            string language = GetSelectedLanguage();
            _plugin.SaveLanguage(language);
            ApplyLanguage();
        }

        private void AddTag(object sender, RoutedEventArgs args)
        {
            AddUnique(_tagList, _tagInput, true);
        }

        private void RemoveTag(object sender, RoutedEventArgs args)
        {
            RemoveSelected(_tagList);
        }

        private void AddIdentityName(object sender, RoutedEventArgs args)
        {
            AddUnique(_identityNameList, _identityNameInput, false);
        }

        private void RemoveIdentityName(object sender, RoutedEventArgs args)
        {
            RemoveSelected(_identityNameList);
        }

        private static void AddUnique(ListBox list, TextBox input, bool upper)
        {
            string value = (input.Text ?? string.Empty).Trim();
            if (upper)
                value = value.ToUpperInvariant();

            if (value.Length == 0)
                return;

            foreach (object item in list.Items)
            {
                if (string.Equals(item.ToString(), value, StringComparison.OrdinalIgnoreCase))
                    return;
            }

            list.Items.Add(value);
            input.Clear();
        }

        private static void RemoveSelected(ListBox list)
        {
            if (list.SelectedItem != null)
                list.Items.Remove(list.SelectedItem);
        }

        private void SaveChanges(object sender, RoutedEventArgs args)
        {
            SaveAllSettings();
            MessageBox.Show(
                BuiltByText.Get(GetSelectedLanguage(), "SavedMessage"),
                BuiltByText.Get(GetSelectedLanguage(), "SavedTitle"),
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void ScanAstronautIdentities(object sender, RoutedEventArgs args)
        {
            SaveAllSettings();
            AstronautCleanupResult result = _plugin.FindAstronautIdentityMatches();
            _cleanupStatus.Text = result.Message;
        }

        private void DeleteAstronautIdentities(object sender, RoutedEventArgs args)
        {
            SaveAllSettings();
            AstronautCleanupResult scan = _plugin.FindAstronautIdentityMatches();
            if (!scan.Success || scan.Found == 0)
            {
                _cleanupStatus.Text = scan.Message;
                return;
            }

            MessageBoxResult answer = MessageBox.Show(
                string.Format(BuiltByText.Get(GetSelectedLanguage(), "ConfirmDelete"), scan.Found),
                BuiltByText.Get(GetSelectedLanguage(), "SavedTitle"),
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (answer != MessageBoxResult.Yes)
                return;

            AstronautCleanupResult result = _plugin.DeleteAstronautIdentityMatches();
            _cleanupStatus.Text = result.Message;
        }

        private void SaveAllSettings()
        {
            _plugin.SaveLanguage(GetSelectedLanguage());

            List<string> tags = _tagList.Items.Cast<object>().Select(x => x.ToString()).ToList();
            _plugin.SaveFactionTags(tags);

            AstronautCleanupConfig cleanup = _plugin.GetAstronautCleanupConfig();
            cleanup.DisplayNames = _identityNameList.Items.Cast<object>().Select(x => x.ToString()).ToList();
            cleanup.AutoCleanupEnabled = _autoCleanupCheckBox.IsChecked == true;
            cleanup.SafeModeEnabled = _safeModeCheckBox.IsChecked != false;
            cleanup.AutoCleanupDelayMinutes = 5;
            _plugin.SaveAstronautCleanupConfig(cleanup);
        }

        private sealed class LanguageItem
        {
            public string Code { get; private set; }
            private readonly string _label;

            public LanguageItem(string code, string label)
            {
                Code = code;
                _label = label;
            }

            public override string ToString()
            {
                return _label;
            }
        }
    }
}
