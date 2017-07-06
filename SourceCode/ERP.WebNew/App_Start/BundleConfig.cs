using System.Web;
using System.Web.Optimization;

namespace ERP.WebNew
{
    public class BundleConfig
    {
        // 有关绑定的详细信息，请访问 http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            System.Web.Optimization.BundleTable.EnableOptimizations = false;

            bundles.Add(new ScriptBundle("~/bundles/CommonJS").Include(
                //"~/Scripts/jquery-{version}.js",
                //"~/Scripts/jquery.easyui-1.4.3.min.js",
                //"~/Scripts/jquery.validate.min.js",
                //"~/Scripts/jquery.validate.unobtrusive.min.js",
                "~/Scripts/jquery.unobtrusive-ajax.min.js",
                "~/Scripts/jquery.cookie.min.js",
                "~/Scripts/jquery.blockui.min.js",
                "~/Content/fancyBox-v2.1.5/source/jquery.fancybox.js",
                "~/Scripts/jquery.slimscroll.min.js",
                //"~/Scripts/bootstrap.js",
                "~/Scripts/respond.js",
                "~/Content/bootstrap-select-1.5.2/bootstrap-select.js",
                "~/Content/bootstrap-validator/js/bootstrapValidator.min.js",
                "~/Content/vakata-jstree-v3.0.0/dist/jstree.min.js",
                "~/Content/DatePicker/WdatePicker.js",
                "~/Scripts/base-loading.js"));

            bundles.Add(new ScriptBundle("~/bundles/AppJS").Include(
                "~/Scripts/app.js",
                "~/Scripts/OA/Init.js",
                "~/Scripts/OA/DataCheck.js",
                "~/Scripts/OA/oa.js"));

            //// 使用要用于开发和学习的 Modernizr 的开发版本。然后，当你做好
            //// 生产准备时，请使用 http://modernizr.com 上的生成工具来仅选择所需的测试。
            //bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
            //    "~/Scripts/modernizr-*"));

            bundles.Add(new StyleBundle("~/bundles/CommonCSS").Include(
                "~/Content/font-awesome-v4.1.0/css/font-awesome.min.css",
                "~/Content/bootstrap/css/bootstrap.min.css",
                "~/Content/fancyBox-v2.1.5/source/jquery.fancybox.css",
                "~/Content/themes/bootstrap/easyui.css",
                "~/Content/bootstrap-select-1.5.2/bootstrap-select.css",
                "~/Content/bootstrap-validator/css/bootstrapValidator.min.css",
                "~/Content/vakata-jstree-v3.0.0/dist/themes/default/style.min.css",
                "~/Content/DatePicker/skin/WdatePicker.css"));

            bundles.Add(new StyleBundle("~/bundles/AppCSS").Include(
                "~/Content/style.css",
                "~/Content/style-responsive.css"));

            // 以下为弹出窗口所需的引用加载
            bundles.Add(new ScriptBundle("~/bundles/popup/CommonJS").Include(
                //"~/Scripts/jquery-{version}.js",
                //"~/Scripts/jquery.easyui-1.4.3.min.js",
                "~/Scripts/jquery.unobtrusive-ajax.min.js",
                //"~/Content/bootstrap/js/bootstrap.min.js",
                "~/Content/Buttons-v1.0/js/buttons.js",
                "~/Content/bootstrap-datetimepicker/js/bootstrap-datetimepicker.min.js",
                "~/Content/bootstrap-datetimepicker/js/locales/bootstrap-datetimepicker.zh-CN.js",
                "~/Scripts/underscore-min.js",
                "~/Scripts/underscore.string.min.js",
                "~/Content/bootstrap-select-1.5.2/bootstrap-select.js",
                "~/Content/bootstrap-daterangepicker/daterangepicker.js",
                "~/Content/bootstrap-validator/js/bootstrapValidator.min.js",
                "~/Content/vakata-jstree-v3.0.0/dist/jstree.min.js",
                //"~/Content/uploadify-v3.0.0/jquery.uploadify.js",
                "~/Content/DatePicker/WdatePicker.js",
                "~/Scripts/base-loading.js"));

            bundles.Add(new ScriptBundle("~/bundles/popup/AppJS").Include(
                "~/Scripts/OA/Init.js",
                "~/Scripts/OA/DataCheck.js",
                "~/Scripts/OA/oa.js"));

            bundles.Add(new StyleBundle("~/bundles/popup/CommonCSS").Include(
                "~/Content/bootstrap/css/bootstrap.min.css",
                "~/Content/font-awesome-v4.1.0/css/font-awesome.min.css",
                "~/Content/Buttons-v1.0/css/buttons.css",
                "~/Content/OA/oa.css",
                "~/Content/bootstrap-datetimepicker/css/bootstrap-datetimepicker.min.css",
                "~/Content/bootstrap-daterangepicker/daterangepicker-bs3.css",
                "~/Content/bootstrap-validator/css/bootstrapValidator.min.css",
                "~/Content/vakata-jstree-v3.0.0/dist/themes/default/style.min.css",
                "~/Content/bootstrap-select-1.5.2/bootstrap-select.css",
                "~/Content/themes/bootstrap/easyui.css",
                "~/Content/uploadify-v3.0.0/uploadify.css",
                "~/Content/DatePicker/skin/WdatePicker.css"));

            bundles.Add(new StyleBundle("~/bundles/popup/AppCSS").Include(
                //"~/Content//fonts/font.css",
                "~/Content/style.css",
                "~/Content/style-responsive.css",
                "~/Content/default.css",
                "~/Content/site.css"));
        }
    }
}