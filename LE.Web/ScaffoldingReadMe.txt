Scaffolding has generated all the files and added the required dependencies.

However the Application's Startup code may required additional changes for things to work end to end.
Add the following code to the Configure method in your Application's Startup class if not already done:

        app.UseMvc(routes =>
        {
          routes.MapRoute(
            name : "areas",
            template : "{area:exists}/{controller=Home}/{action=Index}/{id?}"
          );
        });

        	{
  "ConnectionStrings": {
"DefaultConnection": "server=198.38.83.33;database=leading_test; Integrated Security=False; User ID=leading_bhawesh; Password=nagf5urcdivtbozxyeqp;"  },
  "Logging": {
    "LogLevel": {
      "Default": "Warning"
    }
  },
  "AllowedHosts": "*"
}
