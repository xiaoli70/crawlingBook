using Autofac;
using DeBugWGSD;
using SqlSugar;

namespace WinFormsApp1
{
    internal static class Program
    {
        public static IContainer Container { get; set; }

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            var builder = new ContainerBuilder();

            // Register SqlSugarClient as a singleton
            builder.Register(c => SqlSugarConfig.GetInstance()).As<ISqlSugarClient>().SingleInstance();

            // Register forms
            builder.RegisterType<Form1>().AsSelf().InstancePerDependency();

            Container = builder.Build();



            ApplicationConfiguration.Initialize();
            Application.Run(Container.Resolve<Form1>());
        }
    }
}