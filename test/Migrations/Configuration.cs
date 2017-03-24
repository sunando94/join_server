namespace test.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using test.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<test.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(test.Models.ApplicationDbContext context)
        {
            var categories = new List<Category> { new Category(){CategoryName="Sport",subcategories=new List<SubCategory>{
                new SubCategory(){SubCategoryName="Football"},
                new SubCategory(){SubCategoryName="Hockey"},
                new SubCategory(){SubCategoryName="Running"},
                new SubCategory(){SubCategoryName="Volleyball"},
                new SubCategory(){SubCategoryName="Basketball"},
                new SubCategory(){SubCategoryName="Swimming"},
                new SubCategory(){SubCategoryName="Workout"},
                new SubCategory(){SubCategoryName="Fitness"},
                new SubCategory(){SubCategoryName="Other"}
            }
            },
                new Category(){CategoryName="Entertainment"
                ,subcategories=new List<SubCategory>{

                    new SubCategory(){SubCategoryName="Movie"},new SubCategory(){SubCategoryName="Music"}
                    ,new SubCategory(){SubCategoryName="Art"},new SubCategory(){SubCategoryName="Gaming"},
                    new SubCategory(){SubCategoryName="Gambling"},new SubCategory(){SubCategoryName="Other"}
                    }
                },
                new Category(){CategoryName="Science & Education",subcategories=new List<SubCategory>{
                new SubCategory(){SubCategoryName="Natural Science"},new SubCategory(){SubCategoryName="IT"},
                new SubCategory(){SubCategoryName="Religion"},new SubCategory(){SubCategoryName="Technology"},
                new SubCategory(){SubCategoryName="Political"},new SubCategory(){SubCategoryName="Social Science"},
                new SubCategory(){SubCategoryName="Language"},new SubCategory(){SubCategoryName="Other"}
                }},
                new Category(){CategoryName="LifeStyle",subcategories=new List<SubCategory>{
                new SubCategory(){SubCategoryName="Travel"},new SubCategory(){SubCategoryName="Health"},new SubCategory(){SubCategoryName="Fashion"},
                new SubCategory(){SubCategoryName="Family"},new SubCategory(){SubCategoryName="Other"}
                }},
                new Category(){CategoryName="Hobby",subcategories=new List<SubCategory>{
                new SubCategory(){SubCategoryName="Auto"},new SubCategory(){SubCategoryName="Hunting"},new SubCategory(){SubCategoryName="Animal"},
                new SubCategory(){SubCategoryName="Home"},new SubCategory(){SubCategoryName="Other"}
                }},
                new Category(){CategoryName="Event",subcategories=new List<SubCategory>{
                new SubCategory(){SubCategoryName="Food"},new SubCategory(){SubCategoryName="Party"},new SubCategory(){SubCategoryName="Cafe"},
                new SubCategory(){SubCategoryName="Dating"},new SubCategory(){SubCategoryName="Other"}
                }}
            };

            categories.ForEach(s => context.Categories.AddOrUpdate(p => p.CategoryName, s));
            context.SaveChanges();
        }
    }
}
