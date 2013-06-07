using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Microsoft.VisualBasic;

namespace ConsoleApplication1
{
    internal class GUI : Form
    {
        private readonly List<WriterAction> actions;
        private readonly Button export = new Button();
        private readonly Button insetPageObject = new Button();

        private readonly ListBox list = new ListBox();
        private readonly Button moveDown = new Button();
        private readonly Button moveUp = new Button();
        private readonly Button remove = new Button();
        private readonly TextBox titleField = new TextBox();
        private readonly Label titleLabel = new Label();

        private readonly string[] userActionData = {"Name", "Detected Page", "Node", "Type", "Path", "Text"};
        private readonly List<TextBox> userActionFields = new List<TextBox>();
        private readonly List<Label> userActionLabels = new List<Label>();

        public GUI(List<WriterAction> actions)
        {
            this.actions = actions;
            string latestPageObject = "";
            for(var i = 0; i < actions.Count; i++)
            {
                if (actions[i] is UserAction)
                {
                    if ((actions[i] as UserAction).Page != latestPageObject)
                    {
                        latestPageObject = (actions[i] as UserAction).Page;
                        actions.Insert(i, new PageObjectAction((actions[i] as UserAction).Page));
                    }
                }
            }

            SuspendLayout();

            ClientSize = new Size(800, 600);
            Text = "GUI";

            titleLabel.Text = "Name of your test:";
            titleLabel.Location = new Point(10, 10);

            titleField.Text = Path.ChangeExtension(Path.GetRandomFileName(), null);
            titleField.Location = new Point(200, 10);
            titleField.Width = 190;

            list.Location = new Point(10, 40);
            list.Size = new Size(400, 400);
            list.SelectedIndexChanged += ListSelect;

            moveUp.Text = "Move Up";
            moveUp.Location = new Point(420, 10);
            moveUp.Click += MoveUp;

            moveDown.Text = "Move Down";
            moveDown.Location = new Point(420, 40);
            moveDown.Click += MoveDown;

            remove.Text = "Remove";
            remove.Location = new Point(420, 70);
            remove.Click += Remove;

            int offset = 100;
            foreach (string data in userActionData)
            {
                var l = new Label();
                l.Text = data + ": ";
                l.Location = new Point(420, offset);
                userActionLabels.Add(l);

                var t = new TextBox();
                t.Location = new Point(520, offset);
                t.Width = 300;
                t.LostFocus += UpdateSelected;
                userActionFields.Add(t);

                offset += 30;
            }

            insetPageObject.Text = "Insert Page Object After This";
            insetPageObject.Location = new Point(420, offset);
            insetPageObject.Click += insertPageObject;
            offset += 30;

            export.Text = "Export";
            export.Location = new Point(420, offset);
            export.Click += doExport;
            offset += 30;

            Controls.Add(titleLabel);
            Controls.Add(titleField);
            Controls.Add(list);
            Controls.Add(moveUp);
            Controls.Add(moveDown);
            Controls.Add(remove);

            foreach (Label l in userActionLabels)
            {
                Controls.Add(l);
            }
            foreach (TextBox t in userActionFields)
            {
                Controls.Add(t);
            }

            Controls.Add(insetPageObject);
            Controls.Add(export);

            ResumeLayout();

            Show();
//            this.BringToFront();
//            this.Focus();
            Activate();

            UpdateBox();
        }

        private void MoveUp(Object sender, EventArgs e)
        {
            int index = list.SelectedIndex;
            if (index > 0 && index < actions.Count)
            {
                WriterAction act = actions[index];
                actions.RemoveAt(index);
                actions.Insert(index - 1, act);
                UpdateBox();
            }
        }

        private void MoveDown(Object sender, EventArgs e)
        {
            int index = list.SelectedIndex;
            if (index >= 0 && index < actions.Count - 1)
            {
                WriterAction act = actions[index];
                actions.RemoveAt(index);
                actions.Insert(index + 1, act);
                UpdateBox();
            }
        }

        private void Remove(Object sender, EventArgs e)
        {
            actions.RemoveAt(list.SelectedIndex);
            UpdateBox();
        }

        public static void StartGui(object actions)
        {
            Application.Run(new GUI(actions as List<WriterAction>));
        }

        private void UpdateSelected(Object sender, EventArgs e)
        {
            if (actions[list.SelectedIndex] is UserAction)
            {
                var current = actions[list.SelectedIndex] as UserAction;
                current.Name = userActionFields[0].Text;
                current.Page = userActionFields[1].Text;
                current.Node = userActionFields[2].Text;
                current.Type = userActionFields[3].Text;
                current.Path = userActionFields[4].Text;
                current.Text = userActionFields[5].Text;
            }
            else if (actions[list.SelectedIndex] is PageObjectAction)
            {
                var current = actions[list.SelectedIndex] as PageObjectAction;
                current.Name = userActionFields[0].Text;
            }
            UpdateBox();
        }

        private void UpdateBox()
        {
            list.Items.Clear();
            foreach (WriterAction act in actions)
            {
                list.Items.Add(act.ToString());
            }
        }

        private void ListSelect(Object sender, EventArgs e)
        {
            if (actions[list.SelectedIndex] is UserAction)
            {
                var current = actions[list.SelectedIndex] as UserAction;
                userActionFields[0].Text = current.Name;
                userActionFields[1].Text = current.Page;
                userActionFields[2].Text = current.Node;
                userActionFields[3].Text = current.Type;
                userActionFields[4].Text = current.Path;
                userActionFields[5].Text = current.Text;
            }
            else if (actions[list.SelectedIndex] is PageObjectAction)
            {
                var current = actions[list.SelectedIndex] as PageObjectAction;
                userActionFields[0].Text = current.Name;
                userActionFields[1].Text = "N/A";
                userActionFields[2].Text = "N/A";
                userActionFields[3].Text = "N/A";
                userActionFields[4].Text = "N/A";
                userActionFields[5].Text = "N/A";
            }
        }

        private void doExport(Object sender, EventArgs e)
        {
//            SaveFileDialog dialog = new SaveFileDialog();
//            dialog.Filter = "Class File|*.cs";
//            dialog.Title = "Save the Page Objects";
//            dialog.ShowDialog();
//            string classname = this.titleField.Text;
//
//            this.exportTest(classname);
//            this.exportPageObject(classname);
            Exporter.Export(titleField.Text, actions);

//            System.Text.Encoding.UTF8.GetBytes
//            fs.Write();
        }

        private void insertPageObject(Object sender, EventArgs e)
        {
            string newPageName = Interaction.InputBox("Enter a page object name:", "New Page Object",
                                                      Path.ChangeExtension(Path.GetRandomFileName(), null));
            if (newPageName != "")
            {
                actions.Insert(list.SelectedIndex + 1,
                               new PageObjectAction(newPageName));
                UpdateBox();
            }
        }
    }
}