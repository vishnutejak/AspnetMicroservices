using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discount.API.Extensions
{
    public static class HostExtensions
    {
        public static IHost MigrateDatabase<TContext>(this IHost host, int? retry =0)
        {
            int retryForAvailability = retry.Value;
            
            using(var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var configuration = services.GetRequiredService<IConfiguration>();
                var logger = services.GetRequiredService<ILogger<TContext>>();

                try
                {
                    logger.LogInformation("Starting the DB Migraiton");
                    var connection = new NpgsqlConnection(configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
                    connection.Open();

                    using var command = new NpgsqlCommand
                    {
                        Connection = connection
                    };

                    command.CommandText = "DROP TABLE IF EXISTS COUPON";
                    command.ExecuteNonQuery();

                    command.CommandText = @"CREATE TABLE COUPON (
                                                                Id SERIAL PRIMARY KEY,
                                                                 ProductName VARCHAR(24) NOT NULL,
                                                                 Description TEXT,
                                                                 Amount Int)";
                    command.ExecuteNonQuery();

                    command.CommandText = @"insert into Coupon (ProductName,description, amount) values ('IPhone X', 'Apple Discount',150)";
                    command.ExecuteNonQuery();


                    command.CommandText = @"insert into Coupon (ProductName,description, amount) values ('Samsung 10', 'Samsung Discount',100)";
                    command.ExecuteNonQuery();

                    logger.LogInformation("Migration Completed");
                }
                catch (NpgsqlException ex)
                {

                    logger.LogInformation(ex, "Error while migrating the Discount db database");
                    retryForAvailability++;
                    System.Threading.Thread.Sleep(2000);
                    MigrateDatabase<TContext>(host, retryForAvailability);
                }
            }
            return host;
        }
    }
}
