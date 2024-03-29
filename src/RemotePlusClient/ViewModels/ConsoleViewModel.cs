﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RemotePlusClient.Utils;
using RemotePlusLibrary.SubSystem.Command;

namespace RemotePlusClient.ViewModels
{
    public class ConsoleViewModel : BaseViewModel
    {
        RichTextBox _box = null;
        Label _progressText = null;
        Label _progressLabel = null;
        ProgressBar _progressBar = null;
        SplitContainer _splitPanel = null;
        Color _originalForegroundColor;
        Color _originalBackgroundColor;
        public ConsoleViewModel(string fn, RichTextBox r, Label pt, Label pl, ProgressBar pb, SplitContainer sp) : base(fn)
        {
            _box = r;
            _progressText = pt;
            _progressLabel = pl;
            _progressBar = pb;
            _splitPanel = sp;
            _originalBackgroundColor = _box.BackColor;
            _originalForegroundColor = _box.ForeColor;
        }
        public void Append(string text)
        {
            if(_box.InvokeRequired)
            {
                _box.Invoke(new Action(() => _box.AppendText(text)));
            }
            else
            {
                _box.AppendText(text);
            }
        }
        public void Append(ConsoleText text)
        {
            if(_box.InvokeRequired)
            {
                _box.Invoke(new Action(() => _box.AppendWithColor(text.Text, text.TextColor)));
            }
            else
            {
                _box.AppendWithColor(text.Text, text.TextColor);
            }
        }
        public void AppendLine(string text)
        {
            Append(text + Environment.NewLine);
        }
        public void AppendLine(ConsoleText text)
        {
            Append(text);
            Append(Environment.NewLine);
        }
        public void SetBackgroundColor(Color bgColor)
        {
            _box.BackColor = bgColor;
        }
        public void SetForegroundColor(Color fgColor)
        {
            _box.ForeColor = fgColor;
        }
        public void ResetColors()
        {
            _box.BackColor = _originalBackgroundColor;
            _box.ForeColor = _originalForegroundColor;
        }
        public void ResetText()
        {
            _box.ResetText();
        }
        public void LongOperationStarted(string description, int max)
        {
            _splitPanel.InvokeEx(() => _splitPanel.Panel1Collapsed = false);
            _progressText.InvokeEx(() => _progressText.Text = description);
            _progressBar.InvokeEx(() => _progressBar.Maximum = max);
            _splitPanel.InvokeEx(() => _splitPanel.Panel1.Refresh());
        }
        public void UpdateLongOperation(int value, string progress)
        {
            _progressBar.InvokeEx(() => _progressBar.Value = value);
            _progressLabel.InvokeEx(() => _progressLabel.Text = progress.ToString());
        }
        public void FinishedLongOperation()
        {
            _splitPanel.InvokeEx(() => _splitPanel.Panel1Collapsed = true);
            _progressBar.InvokeEx(() => _progressBar.Value = 0);
            _progressLabel.InvokeEx(() => _progressLabel.Text = "Status");
        }
    }
}