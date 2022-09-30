using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;


namespace Snakes_Client
{
    /// <summary>
    /// Represent an object with UI control that represents its visual apperance.
    /// </summary>
    public abstract class VisualObject
    {

        /// <summary>
        /// Contains all action that will be invoked, when object's apperance is destroyed.
        /// </summary>
        private readonly List<Action> destroyActions;

        /// <summary>
        /// Represent's visual apperance of object.
        /// </summary>
        protected UIElement Visual { get; private set; }

        /// <summary>
        /// Panel that is parent of UIElement that represent object's visual apperance.
        /// </summary>
        protected Panel Panel { get; private set; }

        /// <summary>
        /// Element width.
        /// </summary>
        protected Double Width { get; private set; }

        /// <summary>
        /// Element height.
        /// </summary>

        protected Double Height { get; private set; }

        /// <summary>
        /// Create new visual object.
        /// </summary>
        protected VisualObject()
        {
            destroyActions = new List<Action>();
        }

        /// <summary>
        /// Create new visual apperance for visual object.
        /// </summary>
        /// <returns>Newly created visual apperance.</returns>
        protected abstract UIElement CreateVisual();

        /// <summary>
        /// Create new visual apperance.
        /// </summary>
        /// <param name="panel">Future parent of control thats represents visual apperance.</param>
        /// <param name="width">Element width.</param>
        /// <param name="height">Element height.</param>
        public void CreateVisual(Panel panel, Double width, Double height)
        {
            DestroyVisual();

            Panel = panel;
            Width = width;
            Height = height;

            Visual = CreateVisual();
            if (Visual != null)
                panel.Children.Add(Visual);
        }

        /// <summary>
        /// Add new destroy action, which will occur when object's visual apperance is destroyed.
        /// </summary>
        /// <param name="action">
        /// Action, which will occur when object's visual apperance is destroyed.
        /// </param>
        public void AddDestroyAction(Action action)
        {
            destroyActions.Add(action);
        }

        /// <summary>
        /// Destroy object's visual apperance.
        /// </summary>
        public void DestroyVisual()
        {
            if (Visual != null)
                Panel.Children.Remove(Visual);

            foreach (var action in destroyActions)
                action.Invoke();
            destroyActions.Clear();
        }

    }
}
