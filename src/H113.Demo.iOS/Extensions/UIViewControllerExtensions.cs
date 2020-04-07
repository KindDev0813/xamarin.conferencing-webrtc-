using UIKit;

namespace H113.Demo.iOS.Extensions
{
    public static class UIViewControllerExtensions
    {
        /// <summary>
        /// Moves to this controller to another controller
        /// </summary>
        /// <param name="this">This.</param>
        /// <param name="parent">Parent.</param>
        /// <param name="containerView">Container view if is null will put in parent.view.</param>
        public static void MoveTo(this UIViewController self, UIViewController parent, UIView containerView = null)
        {
            parent.AddChildViewController(self);
            containerView = containerView ??  parent.View;
            containerView.Layer.MasksToBounds = true;
            self.View.Frame = containerView.Bounds;
            self.View.AutoresizingMask = UIViewAutoresizing.FlexibleHeight | UIViewAutoresizing.FlexibleWidth;
            self.View.TranslatesAutoresizingMaskIntoConstraints = true;
            containerView.AddSubview(self.View);

            self.DidMoveToParentViewController(parent);
        }

        public static void RemoveFromParent(this UIViewController self)
        {
            self.WillMoveToParentViewController(null);
            self.View.RemoveFromSuperview();
            self.RemoveFromParentViewController();
        }
    }
}
