﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;
using CH.Alika.POS.TrayApp.Logging;

namespace CH.Alika.POS.TrayApp
{
    // https://www.simple-talk.com/dotnet/.net-framework/creating-tray-applications-in-.net-a-practical-guide/
    class TrayIconApplicationContext : ApplicationContext
    {
        private static readonly string _DefaultTooltip = "Alika Point-Of-Sale";
        private System.ComponentModel.Container components;
        private NotifyIcon notifyIcon;
        private SubscriptionProxy subscriptionProxy;
        private SynchronizationContext _uiThreadContext;

        public TrayIconApplicationContext()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            notifyIcon = new NotifyIcon(components)
            {
                ContextMenuStrip = new ContextMenuStrip(),
                Icon = Properties.Resources.TrayIcon64x64,
                Text = _DefaultTooltip,
                Visible = true
            };

            _uiThreadContext = new WindowsFormsSynchronizationContext();
            notifyIcon.ContextMenuStrip.Opening += ContextMenuStrip_Opening;
            notifyIcon.Click += NotifyIcon_Click;
            // notifyIcon.DoubleClick += notifyIcon_DoubleClick;
            subscriptionProxy = new SubscriptionProxy();
            subscriptionProxy.OnScanEvent += HandleScanEvent;
            subscriptionProxy.OnScanDeliveredEvent += HandleScanDeliveredEvent;
            subscriptionProxy.Activate();
        }

        private void ContextMenuStrip_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = false;
            notifyIcon.ContextMenuStrip.Items.Clear();
            notifyIcon.ContextMenuStrip.Items.Add("Menu 1", null, null);
            notifyIcon.ContextMenuStrip.Items.Add("Menu 2", null, null);
            notifyIcon.ContextMenuStrip.Items.Add("Very Long Menu 3", null, null);
        }

        private void NotifyIcon_Click(object sender, EventArgs e)
        {
            _uiThreadContext.Post((SendOrPostCallback)delegate
            {
                notifyIcon.BalloonTipText = "Will do some action";
                notifyIcon.BalloonTipTitle = "Icon Clicked";
                notifyIcon.BalloonTipIcon = ToolTipIcon.Info;
                notifyIcon.ShowBalloonTip(3);
            }, null);
        }

        private void HandleScanEvent(object source, ScanEvent e)
        {
            _uiThreadContext.Post((SendOrPostCallback)delegate
            {
                notifyIcon.BalloonTipText = "A document was successfully scanned.";
                notifyIcon.BalloonTipTitle = "Document Scanned";
                notifyIcon.ShowBalloonTip(3);
            }, null);
        }

        private void HandleScanDeliveredEvent(object source, ScanDeliveryEvent e)
        {
            _uiThreadContext.Post((SendOrPostCallback)delegate
            {
                System.Media.SystemSounds.Asterisk.Play();
                if (e.ScanDeliveryResult.WasDelivered)
                {
                    notifyIcon.BalloonTipText = "A scanned document was successfully delivered to cloud service";
                    notifyIcon.BalloonTipTitle = "Document Scan Delivered";
                    notifyIcon.BalloonTipIcon = ToolTipIcon.Info;
                }
                else
                {
                    notifyIcon.BalloonTipText = "Error: " + e.ScanDeliveryResult.DeliveryResponse;
                    notifyIcon.BalloonTipTitle = "Document Scan Delivery Failed";
                    notifyIcon.BalloonTipIcon = ToolTipIcon.Error;
                }
                notifyIcon.ShowBalloonTip(3);
            }, null);
        }
    }
}
