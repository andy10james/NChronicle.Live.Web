﻿using Microsoft.AspNetCore.Components;
using NChronicle.Live.Web.Client.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NChronicle.Live.Web.Client.Components.Dialogs
{
    public class Dialog : BaseComponent, IDisposable
    {

        [Parameter] public string Title { get; private set; }
        [Parameter] public RenderFragment ChildContent { get; private set; }
        [Parameter] public Dictionary<string, Action> Buttons { get; private set; }
        [Parameter] public bool Closable { get; private set; }
        [Parameter] public bool UnfocusClosable { get; private set; }
        [Parameter] public bool Modal { get; private set; }
        [Parameter] public bool Show { get; private set; }
        [Parameter] public string ContentClass { get; private set; }
        [Parameter] public Action<Dialog> OnHide { get; private set; }
        [Parameter] public Action<Dialog> OnShow { get; private set; }
        [Parameter] public (byte R, byte G, byte B) AccentColor { get; private set; }

        public event Action<Dialog> OnDialogShow;
        public event Action<Dialog> OnDialogHide;
        public event Action<Dialog> OnDialogUpdated;

        [Inject] private IDialogService dialogService { get; set; }
        private bool previousShowState { get; set; }

        protected override async Task OnInitAsync() =>
            await this.dialogService.RegisterDialogAsync(this);

        protected override async Task OnParametersSetAsync()
        {
            await Task.Run(() => this.OnDialogUpdated?.Invoke(this));
            this.CheckAndNotifyOfShowState();
        }

        public void ShowDialog()
        {
            this.Show = true;
            this.CheckAndNotifyOfShowState();
        }

        public void HideDialog()
        {
            this.Show = false;
            this.CheckAndNotifyOfShowState();
        }

        private void CheckAndNotifyOfShowState()
        {
            if (previousShowState != this.Show)
            {
                Task.Run(() =>
                {
                    if (this.Show)
                    {
                        this.OnDialogShow?.Invoke(this);
                        this.OnShow?.Invoke(this);
                    }
                    else
                    {
                        this.OnDialogHide?.Invoke(this);
                        this.OnHide?.Invoke(this);
                    }
                });
            }
            previousShowState = this.Show;

        }

        #region IDisposable Support
        private bool disposed = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    this.dialogService.UnregisterDialogAsync(this);
                    GC.SuppressFinalize(this);
                }

                disposed = true;
            }
        }

        ~Dialog() => Dispose(false);

        // This code added to correctly implement the disposable pattern.
        public void Dispose() => Dispose(true);
        #endregion

    }
}
