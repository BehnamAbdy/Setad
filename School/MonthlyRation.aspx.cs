using System;
using System.Linq;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Web;
using System.Collections;

public partial class School_MonthlyRation : System.Web.UI.Page
{
    private SupplySystem.SupplySystem db = new SupplySystem.SupplySystem(Public.ConnectionString);
    private const string YELLOW = "#ffff8d";
    private const string RED = "#fd5f35";

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            LoadFoods();
            Public.LoadCycleMonths(this.drpMonths);
            this.lblCycle.Text = Public.ActiveCycle.CycleName;
            if (HttpContext.Current.User.IsInRole("SchoolManager"))
            {
                this.txtSchoolCode.Text = HttpContext.Current.User.Identity.Name;
                this.txtSchoolCode.Enabled = false;
                this.btnSchoolCodeSearch.Enabled = false;
                this.txtSchoolCode_TextChanged(sender, e);
            }

            if (System.Web.HttpContext.Current.User.IsInRole("Guest"))
            {
                this.btnSave.Enabled = false;
            }
        }
    }

    protected void txtSchoolCode_TextChanged(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(this.txtSchoolCode.Text))
        {
            if (!FillHeader())
            {
                ResetHeader();
            }
            else
            {
                this.drpMonths.Enabled = true;
                this.drpMonths.SelectedIndex = 0;
                this.drpSublevel.SelectedIndex = 0;
            }
        }
        else
        {
            ResetHeader();
        }
        ResetDays();
    }

    protected void drpMonths_SelectedIndexChanged(object sender, EventArgs e)
    {
        ResetDays();
        if (this.drpMonths.SelectedIndex > 0)
        {
            this.drpSublevel.Enabled = true;
            string[] monthVals = this.drpMonths.SelectedValue.Split('|');
            short solarYear = Public.ToShort(monthVals[0]);
            byte solarMonth = Public.ToByte(monthVals[1]);
            SetHolidays(solarYear, solarMonth);

            var foodsList = from sf in db.SchoolFoods
                            join cl in db.Calendars on sf.CalendarID equals cl.CalendarID
                            join cf in db.CycleFoods on sf.CycleFoodID equals cf.CycleFoodID
                            where sf.SchoolSubLevelID == Public.ToInt(this.drpSublevel.SelectedValue.Split('|')[0]) &&
                                     !cf.IsDaily && cl.SolarYear == solarYear && cl.SolarMonth == solarMonth
                            select
                            new
                            {
                                sf.CycleFoodID,
                                sf.FoodCount,
                                cl.SolarDay
                            };
            short sum = 0;
            foreach (var item in foodsList)
            {
                SetDayRation(item.SolarDay, item.CycleFoodID, item.FoodCount);
                sum += item.FoodCount;
            }
            this.txtSum.Text = sum.ToString();
        }
    }

    protected void drpSublevel_SelectedIndexChanged(object sender, EventArgs e)
    {
        ResetFoods();
        ResetRations();
        this.lblStudentCount.Text = null;
        if (!string.IsNullOrEmpty(this.hdnSchoolId.Value) && this.drpMonths.SelectedIndex > 0)
        {
            string[] sublevelInfo = this.drpSublevel.SelectedValue.Split('|');
            this.lblStudentCount.Text = sublevelInfo[1];

            string[] monthVals = this.drpMonths.SelectedValue.Split('|');
            short solarYear = Public.ToShort(monthVals[0]);
            byte solarMonth = Public.ToByte(monthVals[1]);

            var rationsList = from sf in db.SchoolFoods
                              join cl in db.Calendars on sf.CalendarID equals cl.CalendarID
                              join cf in db.CycleFoods on sf.CycleFoodID equals cf.CycleFoodID
                              where sf.SchoolSubLevelID == Public.ToInt(sublevelInfo[0]) &&
                                       !cf.IsDaily && cl.SolarYear == solarYear && cl.SolarMonth == solarMonth
                              select
                              new
                              {
                                  sf.CycleFoodID,
                                  sf.FoodCount,
                                  cl.SolarDay
                              };
            short sum = 0;
            foreach (var item in rationsList)
            {
                SetDayRation(item.SolarDay, item.CycleFoodID, item.FoodCount);
                sum += item.FoodCount;
            }
            this.txtSum.Text = sum.ToString();
        }
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        if (Page.IsValid && !string.IsNullOrEmpty(this.hdnSchoolId.Value) && this.drpMonths.Items.Count > 0)
        {
            int schoolSubLevelId = Public.ToInt(this.drpSublevel.SelectedValue.Split('|')[0]);
            string[] monthVals = this.drpMonths.SelectedValue.Split('|');
            short solarYear = Public.ToShort(monthVals[0]);
            byte solarMonth = Public.ToByte(monthVals[1]);

            #region Ration

            SupplySystem.SchoolFood schFood1 = (from sf in db.SchoolFoods
                                                join cl in db.Calendars on sf.CalendarID equals cl.CalendarID
                                                where sf.SchoolSubLevelID == schoolSubLevelId &&
                                                         cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 1
                                                select sf).FirstOrDefault<SupplySystem.SchoolFood>();
            if (schFood1 == null) // Add mode
            {
                if (this.drpStuff1.SelectedIndex > 0 && !string.IsNullOrEmpty(this.txt_1.Text))
                {
                    db.SchoolFoods.InsertOnSubmit(new SupplySystem.SchoolFood
                    {
                        SchoolSubLevelID = schoolSubLevelId,
                        CycleFoodID = Public.ToShort(this.drpStuff1.SelectedValue),
                        CalendarID = db.Calendars.First<SupplySystem.Calendar>(cl => cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 1).CalendarID,
                        FoodCount = Public.ToShort(this.txt_1.Text),
                        SubmitDate = DateTime.Now
                    });
                }
            }
            else // Edit mode
            {
                if (this.drpStuff1.SelectedIndex > 0 && !string.IsNullOrEmpty(this.txt_1.Text))
                {
                    if (schFood1.CycleFoodID.ToString() != this.drpStuff1.SelectedValue) // Delete existing ration and add new one
                    {
                        db.SchoolFoods.DeleteOnSubmit(schFood1);
                        db.SchoolFoods.InsertOnSubmit(new SupplySystem.SchoolFood
                        {
                            SchoolSubLevelID = schoolSubLevelId,
                            CycleFoodID = Public.ToShort(this.drpStuff1.SelectedValue),
                            CalendarID = db.Calendars.First<SupplySystem.Calendar>(cl => cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 1).CalendarID,
                            FoodCount = Public.ToShort(this.txt_1.Text),
                            SubmitDate = DateTime.Now
                        });
                    }
                    else if (schFood1.FoodCount.ToString() != this.txt_1.Text.Trim()) // Edit FoodCount value
                    {
                        schFood1.FoodCount = Public.ToShort(this.txt_1.Text);
                    }
                }
                else if (this.drpStuff1.SelectedIndex == 0 && string.IsNullOrEmpty(this.txt_1.Text)) // Delete ration
                {
                    db.SchoolFoods.DeleteOnSubmit(schFood1);
                }
            }

            SupplySystem.SchoolFood schFood2 = (from sf in db.SchoolFoods
                                                join cl in db.Calendars on sf.CalendarID equals cl.CalendarID
                                                where sf.SchoolSubLevelID == schoolSubLevelId &&
                                                         cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 2
                                                select sf).FirstOrDefault<SupplySystem.SchoolFood>();
            if (schFood2 == null) // Add mode
            {
                if (this.drpStuff2.SelectedIndex > 0 && !string.IsNullOrEmpty(this.txt_2.Text))
                {
                    db.SchoolFoods.InsertOnSubmit(new SupplySystem.SchoolFood
                    {
                        SchoolSubLevelID = schoolSubLevelId,
                        CycleFoodID = Public.ToShort(this.drpStuff2.SelectedValue),
                        CalendarID = db.Calendars.First<SupplySystem.Calendar>(cl => cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 2).CalendarID,
                        FoodCount = Public.ToShort(this.txt_2.Text),
                        SubmitDate = DateTime.Now
                    });
                }
            }
            else // Edit mode
            {
                if (this.drpStuff2.SelectedIndex > 0 && !string.IsNullOrEmpty(this.txt_2.Text))
                {
                    if (schFood2.CycleFoodID.ToString() != this.drpStuff2.SelectedValue) // Delete existing ration and add new one
                    {
                        db.SchoolFoods.DeleteOnSubmit(schFood2);
                        db.SchoolFoods.InsertOnSubmit(new SupplySystem.SchoolFood
                        {
                            SchoolSubLevelID = schoolSubLevelId,
                            CycleFoodID = Public.ToShort(this.drpStuff2.SelectedValue),
                            CalendarID = db.Calendars.First<SupplySystem.Calendar>(cl => cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 2).CalendarID,
                            FoodCount = Public.ToShort(this.txt_2.Text),
                            SubmitDate = DateTime.Now
                        });
                    }
                    else if (schFood2.FoodCount.ToString() != this.txt_2.Text.Trim()) // Edit FoodCount value
                    {
                        schFood2.FoodCount = Public.ToShort(this.txt_2.Text);
                    }
                }
                else if (this.drpStuff2.SelectedIndex == 0 && string.IsNullOrEmpty(this.txt_2.Text)) // Delete ration
                {
                    db.SchoolFoods.DeleteOnSubmit(schFood2);
                }
            }

            SupplySystem.SchoolFood schFood3 = (from sf in db.SchoolFoods
                                                join cl in db.Calendars on sf.CalendarID equals cl.CalendarID
                                                where sf.SchoolSubLevelID == schoolSubLevelId &&
                                                         cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 3
                                                select sf).FirstOrDefault<SupplySystem.SchoolFood>();
            if (schFood3 == null) // Add mode
            {
                if (this.drpStuff3.SelectedIndex > 0 && !string.IsNullOrEmpty(this.txt_3.Text))
                {
                    db.SchoolFoods.InsertOnSubmit(new SupplySystem.SchoolFood
                    {
                        SchoolSubLevelID = schoolSubLevelId,
                        CycleFoodID = Public.ToShort(this.drpStuff3.SelectedValue),
                        CalendarID = db.Calendars.First<SupplySystem.Calendar>(cl => cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 3).CalendarID,
                        FoodCount = Public.ToShort(this.txt_3.Text),
                        SubmitDate = DateTime.Now
                    });
                }
            }
            else // Edit mode
            {
                if (this.drpStuff3.SelectedIndex > 0 && !string.IsNullOrEmpty(this.txt_3.Text))
                {
                    if (schFood3.CycleFoodID.ToString() != this.drpStuff3.SelectedValue) // Delete existing ration and add new one
                    {
                        db.SchoolFoods.DeleteOnSubmit(schFood3);
                        db.SchoolFoods.InsertOnSubmit(new SupplySystem.SchoolFood
                        {
                            SchoolSubLevelID = schoolSubLevelId,
                            CycleFoodID = Public.ToShort(this.drpStuff3.SelectedValue),
                            CalendarID = db.Calendars.First<SupplySystem.Calendar>(cl => cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 3).CalendarID,
                            FoodCount = Public.ToShort(this.txt_3.Text),
                            SubmitDate = DateTime.Now
                        });
                    }
                    else if (schFood3.FoodCount.ToString() != this.txt_3.Text.Trim()) // Edit FoodCount value
                    {
                        schFood3.FoodCount = Public.ToShort(this.txt_3.Text);
                    }
                }
                else if (this.drpStuff3.SelectedIndex == 0 && string.IsNullOrEmpty(this.txt_3.Text)) // Delete ration
                {
                    db.SchoolFoods.DeleteOnSubmit(schFood3);
                }
            }

            SupplySystem.SchoolFood schFood4 = (from sf in db.SchoolFoods
                                                join cl in db.Calendars on sf.CalendarID equals cl.CalendarID
                                                where sf.SchoolSubLevelID == schoolSubLevelId &&
                                                         cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 4
                                                select sf).FirstOrDefault<SupplySystem.SchoolFood>();
            if (schFood4 == null) // Add mode
            {
                if (this.drpStuff4.SelectedIndex > 0 && !string.IsNullOrEmpty(this.txt_4.Text))
                {
                    db.SchoolFoods.InsertOnSubmit(new SupplySystem.SchoolFood
                    {
                        SchoolSubLevelID = schoolSubLevelId,
                        CycleFoodID = Public.ToShort(this.drpStuff4.SelectedValue),
                        CalendarID = db.Calendars.First<SupplySystem.Calendar>(cl => cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 4).CalendarID,
                        FoodCount = Public.ToShort(this.txt_4.Text),
                        SubmitDate = DateTime.Now
                    });
                }
            }
            else // Edit mode
            {
                if (this.drpStuff4.SelectedIndex > 0 && !string.IsNullOrEmpty(this.txt_4.Text))
                {
                    if (schFood4.CycleFoodID.ToString() != this.drpStuff4.SelectedValue) // Delete existing ration and add new one
                    {
                        db.SchoolFoods.DeleteOnSubmit(schFood4);
                        db.SchoolFoods.InsertOnSubmit(new SupplySystem.SchoolFood
                        {
                            SchoolSubLevelID = schoolSubLevelId,
                            CycleFoodID = Public.ToShort(this.drpStuff4.SelectedValue),
                            CalendarID = db.Calendars.First<SupplySystem.Calendar>(cl => cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 4).CalendarID,
                            FoodCount = Public.ToShort(this.txt_4.Text),
                            SubmitDate = DateTime.Now
                        });
                    }
                    else if (schFood4.FoodCount.ToString() != this.txt_4.Text.Trim()) // Edit FoodCount value
                    {
                        schFood4.FoodCount = Public.ToShort(this.txt_4.Text);
                    }
                }
                else if (this.drpStuff4.SelectedIndex == 0 && string.IsNullOrEmpty(this.txt_4.Text)) // Delete ration
                {
                    db.SchoolFoods.DeleteOnSubmit(schFood4);
                }
            }

            SupplySystem.SchoolFood schFood5 = (from sf in db.SchoolFoods
                                                join cl in db.Calendars on sf.CalendarID equals cl.CalendarID
                                                where sf.SchoolSubLevelID == schoolSubLevelId &&
                                                         cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 5
                                                select sf).FirstOrDefault<SupplySystem.SchoolFood>();
            if (schFood5 == null) // Add mode
            {
                if (this.drpStuff5.SelectedIndex > 0 && !string.IsNullOrEmpty(this.txt_5.Text))
                {
                    db.SchoolFoods.InsertOnSubmit(new SupplySystem.SchoolFood
                    {
                        SchoolSubLevelID = schoolSubLevelId,
                        CycleFoodID = Public.ToShort(this.drpStuff5.SelectedValue),
                        CalendarID = db.Calendars.First<SupplySystem.Calendar>(cl => cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 5).CalendarID,
                        FoodCount = Public.ToShort(this.txt_5.Text),
                        SubmitDate = DateTime.Now
                    });
                }
            }
            else // Edit mode
            {
                if (this.drpStuff5.SelectedIndex > 0 && !string.IsNullOrEmpty(this.txt_5.Text))
                {
                    if (schFood5.CycleFoodID.ToString() != this.drpStuff5.SelectedValue) // Delete existing ration and add new one
                    {
                        db.SchoolFoods.DeleteOnSubmit(schFood5);
                        db.SchoolFoods.InsertOnSubmit(new SupplySystem.SchoolFood
                        {
                            SchoolSubLevelID = schoolSubLevelId,
                            CycleFoodID = Public.ToShort(this.drpStuff5.SelectedValue),
                            CalendarID = db.Calendars.First<SupplySystem.Calendar>(cl => cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 5).CalendarID,
                            FoodCount = Public.ToShort(this.txt_5.Text),
                            SubmitDate = DateTime.Now
                        });
                    }
                    else if (schFood5.FoodCount.ToString() != this.txt_5.Text.Trim()) // Edit FoodCount value
                    {
                        schFood5.FoodCount = Public.ToShort(this.txt_5.Text);
                    }
                }
                else if (this.drpStuff5.SelectedIndex == 0 && string.IsNullOrEmpty(this.txt_5.Text)) // Delete ration
                {
                    db.SchoolFoods.DeleteOnSubmit(schFood5);
                }
            }

            SupplySystem.SchoolFood schFood6 = (from sf in db.SchoolFoods
                                                join cl in db.Calendars on sf.CalendarID equals cl.CalendarID
                                                where sf.SchoolSubLevelID == schoolSubLevelId &&
                                                         cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 6
                                                select sf).FirstOrDefault<SupplySystem.SchoolFood>();
            if (schFood6 == null) // Add mode
            {
                if (this.drpStuff6.SelectedIndex > 0 && !string.IsNullOrEmpty(this.txt_6.Text))
                {
                    db.SchoolFoods.InsertOnSubmit(new SupplySystem.SchoolFood
                    {
                        SchoolSubLevelID = schoolSubLevelId,
                        CycleFoodID = Public.ToShort(this.drpStuff6.SelectedValue),
                        CalendarID = db.Calendars.First<SupplySystem.Calendar>(cl => cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 6).CalendarID,
                        FoodCount = Public.ToShort(this.txt_6.Text),
                        SubmitDate = DateTime.Now
                    });
                }
            }
            else // Edit mode
            {
                if (this.drpStuff6.SelectedIndex > 0 && !string.IsNullOrEmpty(this.txt_6.Text))
                {
                    if (schFood6.CycleFoodID.ToString() != this.drpStuff6.SelectedValue) // Delete existing ration and add new one
                    {
                        db.SchoolFoods.DeleteOnSubmit(schFood6);
                        db.SchoolFoods.InsertOnSubmit(new SupplySystem.SchoolFood
                        {
                            SchoolSubLevelID = schoolSubLevelId,
                            CycleFoodID = Public.ToShort(this.drpStuff6.SelectedValue),
                            CalendarID = db.Calendars.First<SupplySystem.Calendar>(cl => cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 6).CalendarID,
                            FoodCount = Public.ToShort(this.txt_6.Text),
                            SubmitDate = DateTime.Now
                        });
                    }
                    else if (schFood6.FoodCount.ToString() != this.txt_6.Text.Trim()) // Edit FoodCount value
                    {
                        schFood6.FoodCount = Public.ToShort(this.txt_6.Text);
                    }
                }
                else if (this.drpStuff6.SelectedIndex == 0 && string.IsNullOrEmpty(this.txt_6.Text)) // Delete ration
                {
                    db.SchoolFoods.DeleteOnSubmit(schFood6);
                }
            }

            SupplySystem.SchoolFood schFood7 = (from sf in db.SchoolFoods
                                                join cl in db.Calendars on sf.CalendarID equals cl.CalendarID
                                                where sf.SchoolSubLevelID == schoolSubLevelId &&
                                                         cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 7
                                                select sf).FirstOrDefault<SupplySystem.SchoolFood>();
            if (schFood7 == null) // Add mode
            {
                if (this.drpStuff7.SelectedIndex > 0 && !string.IsNullOrEmpty(this.txt_7.Text))
                {
                    db.SchoolFoods.InsertOnSubmit(new SupplySystem.SchoolFood
                    {
                        SchoolSubLevelID = schoolSubLevelId,
                        CycleFoodID = Public.ToShort(this.drpStuff7.SelectedValue),
                        CalendarID = db.Calendars.First<SupplySystem.Calendar>(cl => cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 7).CalendarID,
                        FoodCount = Public.ToShort(this.txt_7.Text),
                        SubmitDate = DateTime.Now
                    });
                }
            }
            else // Edit mode
            {
                if (this.drpStuff7.SelectedIndex > 0 && !string.IsNullOrEmpty(this.txt_7.Text))
                {
                    if (schFood7.CycleFoodID.ToString() != this.drpStuff7.SelectedValue) // Delete existing ration and add new one
                    {
                        db.SchoolFoods.DeleteOnSubmit(schFood7);
                        db.SchoolFoods.InsertOnSubmit(new SupplySystem.SchoolFood
                        {
                            SchoolSubLevelID = schoolSubLevelId,
                            CycleFoodID = Public.ToShort(this.drpStuff7.SelectedValue),
                            CalendarID = db.Calendars.First<SupplySystem.Calendar>(cl => cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 7).CalendarID,
                            FoodCount = Public.ToShort(this.txt_7.Text),
                            SubmitDate = DateTime.Now
                        });
                    }
                    else if (schFood7.FoodCount.ToString() != this.txt_7.Text.Trim()) // Edit FoodCount value
                    {
                        schFood7.FoodCount = Public.ToShort(this.txt_7.Text);
                    }
                }
                else if (this.drpStuff7.SelectedIndex == 0 && string.IsNullOrEmpty(this.txt_7.Text)) // Delete ration
                {
                    db.SchoolFoods.DeleteOnSubmit(schFood7);
                }
            }

            SupplySystem.SchoolFood schFood8 = (from sf in db.SchoolFoods
                                                join cl in db.Calendars on sf.CalendarID equals cl.CalendarID
                                                where sf.SchoolSubLevelID == schoolSubLevelId &&
                                                         cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 8
                                                select sf).FirstOrDefault<SupplySystem.SchoolFood>();
            if (schFood8 == null) // Add mode
            {
                if (this.drpStuff8.SelectedIndex > 0 && !string.IsNullOrEmpty(this.txt_8.Text))
                {
                    db.SchoolFoods.InsertOnSubmit(new SupplySystem.SchoolFood
                    {
                        SchoolSubLevelID = schoolSubLevelId,
                        CycleFoodID = Public.ToShort(this.drpStuff8.SelectedValue),
                        CalendarID = db.Calendars.First<SupplySystem.Calendar>(cl => cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 8).CalendarID,
                        FoodCount = Public.ToShort(this.txt_8.Text),
                        SubmitDate = DateTime.Now
                    });
                }
            }
            else // Edit mode
            {
                if (this.drpStuff8.SelectedIndex > 0 && !string.IsNullOrEmpty(this.txt_8.Text))
                {
                    if (schFood8.CycleFoodID.ToString() != this.drpStuff8.SelectedValue) // Delete existing ration and add new one
                    {
                        db.SchoolFoods.DeleteOnSubmit(schFood8);
                        db.SchoolFoods.InsertOnSubmit(new SupplySystem.SchoolFood
                        {
                            SchoolSubLevelID = schoolSubLevelId,
                            CycleFoodID = Public.ToShort(this.drpStuff8.SelectedValue),
                            CalendarID = db.Calendars.First<SupplySystem.Calendar>(cl => cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 8).CalendarID,
                            FoodCount = Public.ToShort(this.txt_8.Text),
                            SubmitDate = DateTime.Now
                        });
                    }
                    else if (schFood8.FoodCount.ToString() != this.txt_8.Text.Trim()) // Edit FoodCount value
                    {
                        schFood8.FoodCount = Public.ToShort(this.txt_8.Text);
                    }
                }
                else if (this.drpStuff8.SelectedIndex == 0 && string.IsNullOrEmpty(this.txt_8.Text)) // Delete ration
                {
                    db.SchoolFoods.DeleteOnSubmit(schFood8);
                }
            }

            SupplySystem.SchoolFood schFood9 = (from sf in db.SchoolFoods
                                                join cl in db.Calendars on sf.CalendarID equals cl.CalendarID
                                                where sf.SchoolSubLevelID == schoolSubLevelId &&
                                                         cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 9
                                                select sf).FirstOrDefault<SupplySystem.SchoolFood>();
            if (schFood9 == null) // Add mode
            {
                if (this.drpStuff9.SelectedIndex > 0 && !string.IsNullOrEmpty(this.txt_9.Text))
                {
                    db.SchoolFoods.InsertOnSubmit(new SupplySystem.SchoolFood
                    {
                        SchoolSubLevelID = schoolSubLevelId,
                        CycleFoodID = Public.ToShort(this.drpStuff9.SelectedValue),
                        CalendarID = db.Calendars.First<SupplySystem.Calendar>(cl => cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 9).CalendarID,
                        FoodCount = Public.ToShort(this.txt_9.Text),
                        SubmitDate = DateTime.Now
                    });
                }
            }
            else // Edit mode
            {
                if (this.drpStuff9.SelectedIndex > 0 && !string.IsNullOrEmpty(this.txt_9.Text))
                {
                    if (schFood9.CycleFoodID.ToString() != this.drpStuff9.SelectedValue) // Delete existing ration and add new one
                    {
                        db.SchoolFoods.DeleteOnSubmit(schFood9);
                        db.SchoolFoods.InsertOnSubmit(new SupplySystem.SchoolFood
                        {
                            SchoolSubLevelID = schoolSubLevelId,
                            CycleFoodID = Public.ToShort(this.drpStuff9.SelectedValue),
                            CalendarID = db.Calendars.First<SupplySystem.Calendar>(cl => cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 9).CalendarID,
                            FoodCount = Public.ToShort(this.txt_9.Text),
                            SubmitDate = DateTime.Now
                        });
                    }
                    else if (schFood9.FoodCount.ToString() != this.txt_9.Text.Trim()) // Edit FoodCount value
                    {
                        schFood9.FoodCount = Public.ToShort(this.txt_9.Text);
                    }
                }
                else if (this.drpStuff9.SelectedIndex == 0 && string.IsNullOrEmpty(this.txt_9.Text)) // Delete ration
                {
                    db.SchoolFoods.DeleteOnSubmit(schFood9);
                }
            }

            SupplySystem.SchoolFood schFood10 = (from sf in db.SchoolFoods
                                                 join cl in db.Calendars on sf.CalendarID equals cl.CalendarID
                                                 where sf.SchoolSubLevelID == schoolSubLevelId &&
                                                          cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 10
                                                 select sf).FirstOrDefault<SupplySystem.SchoolFood>();
            if (schFood10 == null) // Add mode
            {
                if (this.drpStuff10.SelectedIndex > 0 && !string.IsNullOrEmpty(this.txt_10.Text))
                {
                    db.SchoolFoods.InsertOnSubmit(new SupplySystem.SchoolFood
                    {
                        SchoolSubLevelID = schoolSubLevelId,
                        CycleFoodID = Public.ToShort(this.drpStuff10.SelectedValue),
                        CalendarID = db.Calendars.First<SupplySystem.Calendar>(cl => cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 10).CalendarID,
                        FoodCount = Public.ToShort(this.txt_10.Text),
                        SubmitDate = DateTime.Now
                    });
                }
            }
            else // Edit mode
            {
                if (this.drpStuff10.SelectedIndex > 0 && !string.IsNullOrEmpty(this.txt_10.Text))
                {
                    if (schFood10.CycleFoodID.ToString() != this.drpStuff10.SelectedValue) // Delete existing ration and add new one
                    {
                        db.SchoolFoods.DeleteOnSubmit(schFood10);
                        db.SchoolFoods.InsertOnSubmit(new SupplySystem.SchoolFood
                        {
                            SchoolSubLevelID = schoolSubLevelId,
                            CycleFoodID = Public.ToShort(this.drpStuff10.SelectedValue),
                            CalendarID = db.Calendars.First<SupplySystem.Calendar>(cl => cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 10).CalendarID,
                            FoodCount = Public.ToShort(this.txt_10.Text),
                            SubmitDate = DateTime.Now
                        });
                    }
                    else if (schFood10.FoodCount.ToString() != this.txt_10.Text.Trim()) // Edit FoodCount value
                    {
                        schFood10.FoodCount = Public.ToShort(this.txt_10.Text);
                    }
                }
                else if (this.drpStuff10.SelectedIndex == 0 && string.IsNullOrEmpty(this.txt_10.Text)) // Delete ration
                {
                    db.SchoolFoods.DeleteOnSubmit(schFood10);
                }
            }

            SupplySystem.SchoolFood schFood11 = (from sf in db.SchoolFoods
                                                 join cl in db.Calendars on sf.CalendarID equals cl.CalendarID
                                                 where sf.SchoolSubLevelID == schoolSubLevelId &&
                                                          cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 11
                                                 select sf).FirstOrDefault<SupplySystem.SchoolFood>();
            if (schFood11 == null) // Add mode
            {
                if (this.drpStuff11.SelectedIndex > 0 && !string.IsNullOrEmpty(this.txt_11.Text))
                {
                    db.SchoolFoods.InsertOnSubmit(new SupplySystem.SchoolFood
                    {
                        SchoolSubLevelID = schoolSubLevelId,
                        CycleFoodID = Public.ToShort(this.drpStuff11.SelectedValue),
                        CalendarID = db.Calendars.First<SupplySystem.Calendar>(cl => cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 11).CalendarID,
                        FoodCount = Public.ToShort(this.txt_11.Text),
                        SubmitDate = DateTime.Now
                    });
                }
            }
            else // Edit mode
            {
                if (this.drpStuff11.SelectedIndex > 0 && !string.IsNullOrEmpty(this.txt_11.Text))
                {
                    if (schFood11.CycleFoodID.ToString() != this.drpStuff11.SelectedValue) // Delete existing ration and add new one
                    {
                        db.SchoolFoods.DeleteOnSubmit(schFood11);
                        db.SchoolFoods.InsertOnSubmit(new SupplySystem.SchoolFood
                        {
                            SchoolSubLevelID = schoolSubLevelId,
                            CycleFoodID = Public.ToShort(this.drpStuff11.SelectedValue),
                            CalendarID = db.Calendars.First<SupplySystem.Calendar>(cl => cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 11).CalendarID,
                            FoodCount = Public.ToShort(this.txt_11.Text),
                            SubmitDate = DateTime.Now
                        });
                    }
                    else if (schFood11.FoodCount.ToString() != this.txt_11.Text.Trim()) // Edit FoodCount value
                    {
                        schFood11.FoodCount = Public.ToShort(this.txt_11.Text);
                    }
                }
                else if (this.drpStuff11.SelectedIndex == 0 && string.IsNullOrEmpty(this.txt_11.Text)) // Delete ration
                {
                    db.SchoolFoods.DeleteOnSubmit(schFood11);
                }
            }

            SupplySystem.SchoolFood schFood12 = (from sf in db.SchoolFoods
                                                 join cl in db.Calendars on sf.CalendarID equals cl.CalendarID
                                                 where sf.SchoolSubLevelID == schoolSubLevelId &&
                                                          cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 12
                                                 select sf).FirstOrDefault<SupplySystem.SchoolFood>();
            if (schFood12 == null) // Add mode
            {
                if (this.drpStuff12.SelectedIndex > 0 && !string.IsNullOrEmpty(this.txt_12.Text))
                {
                    db.SchoolFoods.InsertOnSubmit(new SupplySystem.SchoolFood
                    {
                        SchoolSubLevelID = schoolSubLevelId,
                        CycleFoodID = Public.ToShort(this.drpStuff12.SelectedValue),
                        CalendarID = db.Calendars.First<SupplySystem.Calendar>(cl => cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 12).CalendarID,
                        FoodCount = Public.ToShort(this.txt_12.Text),
                        SubmitDate = DateTime.Now
                    });
                }
            }
            else // Edit mode
            {
                if (this.drpStuff12.SelectedIndex > 0 && !string.IsNullOrEmpty(this.txt_12.Text))
                {
                    if (schFood12.CycleFoodID.ToString() != this.drpStuff12.SelectedValue) // Delete existing ration and add new one
                    {
                        db.SchoolFoods.DeleteOnSubmit(schFood12);
                        db.SchoolFoods.InsertOnSubmit(new SupplySystem.SchoolFood
                        {
                            SchoolSubLevelID = schoolSubLevelId,
                            CycleFoodID = Public.ToShort(this.drpStuff12.SelectedValue),
                            CalendarID = db.Calendars.First<SupplySystem.Calendar>(cl => cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 12).CalendarID,
                            FoodCount = Public.ToShort(this.txt_12.Text),
                            SubmitDate = DateTime.Now
                        });
                    }
                    else if (schFood12.FoodCount.ToString() != this.txt_12.Text.Trim()) // Edit FoodCount value
                    {
                        schFood12.FoodCount = Public.ToShort(this.txt_12.Text);
                    }
                }
                else if (this.drpStuff12.SelectedIndex == 0 && string.IsNullOrEmpty(this.txt_12.Text)) // Delete ration
                {
                    db.SchoolFoods.DeleteOnSubmit(schFood12);
                }
            }

            SupplySystem.SchoolFood schFood13 = (from sf in db.SchoolFoods
                                                 join cl in db.Calendars on sf.CalendarID equals cl.CalendarID
                                                 where sf.SchoolSubLevelID == schoolSubLevelId &&
                                                          cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 13
                                                 select sf).FirstOrDefault<SupplySystem.SchoolFood>();
            if (schFood13 == null) // Add mode
            {
                if (this.drpStuff13.SelectedIndex > 0 && !string.IsNullOrEmpty(this.txt_13.Text))
                {
                    db.SchoolFoods.InsertOnSubmit(new SupplySystem.SchoolFood
                    {
                        SchoolSubLevelID = schoolSubLevelId,
                        CycleFoodID = Public.ToShort(this.drpStuff13.SelectedValue),
                        CalendarID = db.Calendars.First<SupplySystem.Calendar>(cl => cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 13).CalendarID,
                        FoodCount = Public.ToShort(this.txt_13.Text),
                        SubmitDate = DateTime.Now
                    });
                }
            }
            else // Edit mode
            {
                if (this.drpStuff13.SelectedIndex > 0 && !string.IsNullOrEmpty(this.txt_13.Text))
                {
                    if (schFood13.CycleFoodID.ToString() != this.drpStuff13.SelectedValue) // Delete existing ration and add new one
                    {
                        db.SchoolFoods.DeleteOnSubmit(schFood13);
                        db.SchoolFoods.InsertOnSubmit(new SupplySystem.SchoolFood
                        {
                            SchoolSubLevelID = schoolSubLevelId,
                            CycleFoodID = Public.ToShort(this.drpStuff13.SelectedValue),
                            CalendarID = db.Calendars.First<SupplySystem.Calendar>(cl => cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 13).CalendarID,
                            FoodCount = Public.ToShort(this.txt_13.Text),
                            SubmitDate = DateTime.Now
                        });
                    }
                    else if (schFood13.FoodCount.ToString() != this.txt_13.Text.Trim()) // Edit FoodCount value
                    {
                        schFood13.FoodCount = Public.ToShort(this.txt_13.Text);
                    }
                }
                else if (this.drpStuff13.SelectedIndex == 0 && string.IsNullOrEmpty(this.txt_13.Text)) // Delete ration
                {
                    db.SchoolFoods.DeleteOnSubmit(schFood13);
                }
            }

            SupplySystem.SchoolFood schFood14 = (from sf in db.SchoolFoods
                                                 join cl in db.Calendars on sf.CalendarID equals cl.CalendarID
                                                 where sf.SchoolSubLevelID == schoolSubLevelId &&
                                                          cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 14
                                                 select sf).FirstOrDefault<SupplySystem.SchoolFood>();
            if (schFood14 == null) // Add mode
            {
                if (this.drpStuff14.SelectedIndex > 0 && !string.IsNullOrEmpty(this.txt_14.Text))
                {
                    db.SchoolFoods.InsertOnSubmit(new SupplySystem.SchoolFood
                    {
                        SchoolSubLevelID = schoolSubLevelId,
                        CycleFoodID = Public.ToShort(this.drpStuff14.SelectedValue),
                        CalendarID = db.Calendars.First<SupplySystem.Calendar>(cl => cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 14).CalendarID,
                        FoodCount = Public.ToShort(this.txt_14.Text),
                        SubmitDate = DateTime.Now
                    });
                }
            }
            else // Edit mode
            {
                if (this.drpStuff14.SelectedIndex > 0 && !string.IsNullOrEmpty(this.txt_14.Text))
                {
                    if (schFood14.CycleFoodID.ToString() != this.drpStuff14.SelectedValue) // Delete existing ration and add new one
                    {
                        db.SchoolFoods.DeleteOnSubmit(schFood14);
                        db.SchoolFoods.InsertOnSubmit(new SupplySystem.SchoolFood
                        {
                            SchoolSubLevelID = schoolSubLevelId,
                            CycleFoodID = Public.ToShort(this.drpStuff14.SelectedValue),
                            CalendarID = db.Calendars.First<SupplySystem.Calendar>(cl => cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 14).CalendarID,
                            FoodCount = Public.ToShort(this.txt_14.Text),
                            SubmitDate = DateTime.Now
                        });
                    }
                    else if (schFood14.FoodCount.ToString() != this.txt_14.Text.Trim()) // Edit FoodCount value
                    {
                        schFood14.FoodCount = Public.ToShort(this.txt_14.Text);
                    }
                }
                else if (this.drpStuff14.SelectedIndex == 0 && string.IsNullOrEmpty(this.txt_14.Text)) // Delete ration
                {
                    db.SchoolFoods.DeleteOnSubmit(schFood14);
                }
            }

            SupplySystem.SchoolFood schFood15 = (from sf in db.SchoolFoods
                                                 join cl in db.Calendars on sf.CalendarID equals cl.CalendarID
                                                 where sf.SchoolSubLevelID == schoolSubLevelId &&
                                                          cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 15
                                                 select sf).FirstOrDefault<SupplySystem.SchoolFood>();
            if (schFood15 == null) // Add mode
            {
                if (this.drpStuff15.SelectedIndex > 0 && !string.IsNullOrEmpty(this.txt_15.Text))
                {
                    db.SchoolFoods.InsertOnSubmit(new SupplySystem.SchoolFood
                    {
                        SchoolSubLevelID = schoolSubLevelId,
                        CycleFoodID = Public.ToShort(this.drpStuff15.SelectedValue),
                        CalendarID = db.Calendars.First<SupplySystem.Calendar>(cl => cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 15).CalendarID,
                        FoodCount = Public.ToShort(this.txt_15.Text),
                        SubmitDate = DateTime.Now
                    });
                }
            }
            else // Edit mode
            {
                if (this.drpStuff15.SelectedIndex > 0 && !string.IsNullOrEmpty(this.txt_15.Text))
                {
                    if (schFood15.CycleFoodID.ToString() != this.drpStuff15.SelectedValue) // Delete existing ration and add new one
                    {
                        db.SchoolFoods.DeleteOnSubmit(schFood15);
                        db.SchoolFoods.InsertOnSubmit(new SupplySystem.SchoolFood
                        {
                            SchoolSubLevelID = schoolSubLevelId,
                            CycleFoodID = Public.ToShort(this.drpStuff15.SelectedValue),
                            CalendarID = db.Calendars.First<SupplySystem.Calendar>(cl => cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 15).CalendarID,
                            FoodCount = Public.ToShort(this.txt_15.Text),
                            SubmitDate = DateTime.Now
                        });
                    }
                    else if (schFood15.FoodCount.ToString() != this.txt_15.Text.Trim()) // Edit FoodCount value
                    {
                        schFood15.FoodCount = Public.ToShort(this.txt_15.Text);
                    }
                }
                else if (this.drpStuff15.SelectedIndex == 0 && string.IsNullOrEmpty(this.txt_15.Text)) // Delete ration
                {
                    db.SchoolFoods.DeleteOnSubmit(schFood15);
                }
            }

            SupplySystem.SchoolFood schFood16 = (from sf in db.SchoolFoods
                                                 join cl in db.Calendars on sf.CalendarID equals cl.CalendarID
                                                 where sf.SchoolSubLevelID == schoolSubLevelId &&
                                                          cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 16
                                                 select sf).FirstOrDefault<SupplySystem.SchoolFood>();
            if (schFood16 == null) // Add mode
            {
                if (this.drpStuff16.SelectedIndex > 0 && !string.IsNullOrEmpty(this.txt_16.Text))
                {
                    db.SchoolFoods.InsertOnSubmit(new SupplySystem.SchoolFood
                    {
                        SchoolSubLevelID = schoolSubLevelId,
                        CycleFoodID = Public.ToShort(this.drpStuff16.SelectedValue),
                        CalendarID = db.Calendars.First<SupplySystem.Calendar>(cl => cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 16).CalendarID,
                        FoodCount = Public.ToShort(this.txt_16.Text),
                        SubmitDate = DateTime.Now
                    });
                }
            }
            else // Edit mode
            {
                if (this.drpStuff16.SelectedIndex > 0 && !string.IsNullOrEmpty(this.txt_16.Text))
                {
                    if (schFood16.CycleFoodID.ToString() != this.drpStuff16.SelectedValue) // Delete existing ration and add new one
                    {
                        db.SchoolFoods.DeleteOnSubmit(schFood16);
                        db.SchoolFoods.InsertOnSubmit(new SupplySystem.SchoolFood
                        {
                            SchoolSubLevelID = schoolSubLevelId,
                            CycleFoodID = Public.ToShort(this.drpStuff16.SelectedValue),
                            CalendarID = db.Calendars.First<SupplySystem.Calendar>(cl => cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 16).CalendarID,
                            FoodCount = Public.ToShort(this.txt_16.Text),
                            SubmitDate = DateTime.Now
                        });
                    }
                    else if (schFood16.FoodCount.ToString() != this.txt_16.Text.Trim()) // Edit FoodCount value
                    {
                        schFood16.FoodCount = Public.ToShort(this.txt_16.Text);
                    }
                }
                else if (this.drpStuff16.SelectedIndex == 0 && string.IsNullOrEmpty(this.txt_16.Text)) // Delete ration
                {
                    db.SchoolFoods.DeleteOnSubmit(schFood16);
                }
            }

            SupplySystem.SchoolFood schFood17 = (from sf in db.SchoolFoods
                                                 join cl in db.Calendars on sf.CalendarID equals cl.CalendarID
                                                 where sf.SchoolSubLevelID == schoolSubLevelId &&
                                                          cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 17
                                                 select sf).FirstOrDefault<SupplySystem.SchoolFood>();
            if (schFood17 == null) // Add mode
            {
                if (this.drpStuff17.SelectedIndex > 0 && !string.IsNullOrEmpty(this.txt_17.Text))
                {
                    db.SchoolFoods.InsertOnSubmit(new SupplySystem.SchoolFood
                    {
                        SchoolSubLevelID = schoolSubLevelId,
                        CycleFoodID = Public.ToShort(this.drpStuff17.SelectedValue),
                        CalendarID = db.Calendars.First<SupplySystem.Calendar>(cl => cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 17).CalendarID,
                        FoodCount = Public.ToShort(this.txt_17.Text),
                        SubmitDate = DateTime.Now
                    });
                }
            }
            else // Edit mode
            {
                if (this.drpStuff17.SelectedIndex > 0 && !string.IsNullOrEmpty(this.txt_17.Text))
                {
                    if (schFood17.CycleFoodID.ToString() != this.drpStuff17.SelectedValue) // Delete existing ration and add new one
                    {
                        db.SchoolFoods.DeleteOnSubmit(schFood17);
                        db.SchoolFoods.InsertOnSubmit(new SupplySystem.SchoolFood
                        {
                            SchoolSubLevelID = schoolSubLevelId,
                            CycleFoodID = Public.ToShort(this.drpStuff17.SelectedValue),
                            CalendarID = db.Calendars.First<SupplySystem.Calendar>(cl => cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 17).CalendarID,
                            FoodCount = Public.ToShort(this.txt_17.Text),
                            SubmitDate = DateTime.Now
                        });
                    }
                    else if (schFood17.FoodCount.ToString() != this.txt_17.Text.Trim()) // Edit FoodCount value
                    {
                        schFood17.FoodCount = Public.ToShort(this.txt_17.Text);
                    }
                }
                else if (this.drpStuff17.SelectedIndex == 0 && string.IsNullOrEmpty(this.txt_17.Text)) // Delete ration
                {
                    db.SchoolFoods.DeleteOnSubmit(schFood17);
                }
            }

            SupplySystem.SchoolFood schFood18 = (from sf in db.SchoolFoods
                                                 join cl in db.Calendars on sf.CalendarID equals cl.CalendarID
                                                 where sf.SchoolSubLevelID == schoolSubLevelId &&
                                                          cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 18
                                                 select sf).FirstOrDefault<SupplySystem.SchoolFood>();
            if (schFood18 == null) // Add mode
            {
                if (this.drpStuff18.SelectedIndex > 0 && !string.IsNullOrEmpty(this.txt_18.Text))
                {
                    db.SchoolFoods.InsertOnSubmit(new SupplySystem.SchoolFood
                    {
                        SchoolSubLevelID = schoolSubLevelId,
                        CycleFoodID = Public.ToShort(this.drpStuff18.SelectedValue),
                        CalendarID = db.Calendars.First<SupplySystem.Calendar>(cl => cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 18).CalendarID,
                        FoodCount = Public.ToShort(this.txt_18.Text),
                        SubmitDate = DateTime.Now
                    });
                }
            }
            else // Edit mode
            {
                if (this.drpStuff18.SelectedIndex > 0 && !string.IsNullOrEmpty(this.txt_18.Text))
                {
                    if (schFood18.CycleFoodID.ToString() != this.drpStuff18.SelectedValue) // Delete existing ration and add new one
                    {
                        db.SchoolFoods.DeleteOnSubmit(schFood18);
                        db.SchoolFoods.InsertOnSubmit(new SupplySystem.SchoolFood
                        {
                            SchoolSubLevelID = schoolSubLevelId,
                            CycleFoodID = Public.ToShort(this.drpStuff18.SelectedValue),
                            CalendarID = db.Calendars.First<SupplySystem.Calendar>(cl => cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 18).CalendarID,
                            FoodCount = Public.ToShort(this.txt_18.Text),
                            SubmitDate = DateTime.Now
                        });
                    }
                    else if (schFood18.FoodCount.ToString() != this.txt_18.Text.Trim()) // Edit FoodCount value
                    {
                        schFood18.FoodCount = Public.ToShort(this.txt_18.Text);
                    }
                }
                else if (this.drpStuff18.SelectedIndex == 0 && string.IsNullOrEmpty(this.txt_18.Text)) // Delete ration
                {
                    db.SchoolFoods.DeleteOnSubmit(schFood18);
                }
            }

            SupplySystem.SchoolFood schFood19 = (from sf in db.SchoolFoods
                                                 join cl in db.Calendars on sf.CalendarID equals cl.CalendarID
                                                 where sf.SchoolSubLevelID == schoolSubLevelId &&
                                                          cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 19
                                                 select sf).FirstOrDefault<SupplySystem.SchoolFood>();
            if (schFood19 == null) // Add mode
            {
                if (this.drpStuff19.SelectedIndex > 0 && !string.IsNullOrEmpty(this.txt_19.Text))
                {
                    db.SchoolFoods.InsertOnSubmit(new SupplySystem.SchoolFood
                    {
                        SchoolSubLevelID = schoolSubLevelId,
                        CycleFoodID = Public.ToShort(this.drpStuff19.SelectedValue),
                        CalendarID = db.Calendars.First<SupplySystem.Calendar>(cl => cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 19).CalendarID,
                        FoodCount = Public.ToShort(this.txt_19.Text),
                        SubmitDate = DateTime.Now
                    });
                }
            }
            else // Edit mode
            {
                if (this.drpStuff19.SelectedIndex > 0 && !string.IsNullOrEmpty(this.txt_19.Text))
                {
                    if (schFood19.CycleFoodID.ToString() != this.drpStuff19.SelectedValue) // Delete existing ration and add new one
                    {
                        db.SchoolFoods.DeleteOnSubmit(schFood19);
                        db.SchoolFoods.InsertOnSubmit(new SupplySystem.SchoolFood
                        {
                            SchoolSubLevelID = schoolSubLevelId,
                            CycleFoodID = Public.ToShort(this.drpStuff19.SelectedValue),
                            CalendarID = db.Calendars.First<SupplySystem.Calendar>(cl => cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 19).CalendarID,
                            FoodCount = Public.ToShort(this.txt_19.Text),
                            SubmitDate = DateTime.Now
                        });
                    }
                    else if (schFood19.FoodCount.ToString() != this.txt_19.Text.Trim()) // Edit FoodCount value
                    {
                        schFood19.FoodCount = Public.ToShort(this.txt_19.Text);
                    }
                }
                else if (this.drpStuff19.SelectedIndex == 0 && string.IsNullOrEmpty(this.txt_19.Text)) // Delete ration
                {
                    db.SchoolFoods.DeleteOnSubmit(schFood19);
                }
            }

            SupplySystem.SchoolFood schFood20 = (from sf in db.SchoolFoods
                                                 join cl in db.Calendars on sf.CalendarID equals cl.CalendarID
                                                 where sf.SchoolSubLevelID == schoolSubLevelId &&
                                                          cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 20
                                                 select sf).FirstOrDefault<SupplySystem.SchoolFood>();
            if (schFood20 == null) // Add mode
            {
                if (this.drpStuff20.SelectedIndex > 0 && !string.IsNullOrEmpty(this.txt_20.Text))
                {
                    db.SchoolFoods.InsertOnSubmit(new SupplySystem.SchoolFood
                    {
                        SchoolSubLevelID = schoolSubLevelId,
                        CycleFoodID = Public.ToShort(this.drpStuff20.SelectedValue),
                        CalendarID = db.Calendars.First<SupplySystem.Calendar>(cl => cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 20).CalendarID,
                        FoodCount = Public.ToShort(this.txt_20.Text),
                        SubmitDate = DateTime.Now
                    });
                }
            }
            else // Edit mode
            {
                if (this.drpStuff20.SelectedIndex > 0 && !string.IsNullOrEmpty(this.txt_20.Text))
                {
                    if (schFood20.CycleFoodID.ToString() != this.drpStuff20.SelectedValue) // Delete existing ration and add new one
                    {
                        db.SchoolFoods.DeleteOnSubmit(schFood20);
                        db.SchoolFoods.InsertOnSubmit(new SupplySystem.SchoolFood
                        {
                            SchoolSubLevelID = schoolSubLevelId,
                            CycleFoodID = Public.ToShort(this.drpStuff20.SelectedValue),
                            CalendarID = db.Calendars.First<SupplySystem.Calendar>(cl => cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 20).CalendarID,
                            FoodCount = Public.ToShort(this.txt_20.Text),
                            SubmitDate = DateTime.Now
                        });
                    }
                    else if (schFood20.FoodCount.ToString() != this.txt_20.Text.Trim()) // Edit FoodCount value
                    {
                        schFood20.FoodCount = Public.ToShort(this.txt_20.Text);
                    }
                }
                else if (this.drpStuff20.SelectedIndex == 0 && string.IsNullOrEmpty(this.txt_20.Text)) // Delete ration
                {
                    db.SchoolFoods.DeleteOnSubmit(schFood20);
                }
            }

            SupplySystem.SchoolFood schFood21 = (from sf in db.SchoolFoods
                                                 join cl in db.Calendars on sf.CalendarID equals cl.CalendarID
                                                 where sf.SchoolSubLevelID == schoolSubLevelId &&
                                                          cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 21
                                                 select sf).FirstOrDefault<SupplySystem.SchoolFood>();
            if (schFood21 == null) // Add mode
            {
                if (this.drpStuff21.SelectedIndex > 0 && !string.IsNullOrEmpty(this.txt_21.Text))
                {
                    db.SchoolFoods.InsertOnSubmit(new SupplySystem.SchoolFood
                    {
                        SchoolSubLevelID = schoolSubLevelId,
                        CycleFoodID = Public.ToShort(this.drpStuff21.SelectedValue),
                        CalendarID = db.Calendars.First<SupplySystem.Calendar>(cl => cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 21).CalendarID,
                        FoodCount = Public.ToShort(this.txt_21.Text),
                        SubmitDate = DateTime.Now
                    });
                }
            }
            else // Edit mode
            {
                if (this.drpStuff21.SelectedIndex > 0 && !string.IsNullOrEmpty(this.txt_21.Text))
                {
                    if (schFood21.CycleFoodID.ToString() != this.drpStuff21.SelectedValue) // Delete existing ration and add new one
                    {
                        db.SchoolFoods.DeleteOnSubmit(schFood21);
                        db.SchoolFoods.InsertOnSubmit(new SupplySystem.SchoolFood
                        {
                            SchoolSubLevelID = schoolSubLevelId,
                            CycleFoodID = Public.ToShort(this.drpStuff21.SelectedValue),
                            CalendarID = db.Calendars.First<SupplySystem.Calendar>(cl => cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 21).CalendarID,
                            FoodCount = Public.ToShort(this.txt_21.Text),
                            SubmitDate = DateTime.Now
                        });
                    }
                    else if (schFood21.FoodCount.ToString() != this.txt_21.Text.Trim()) // Edit FoodCount value
                    {
                        schFood21.FoodCount = Public.ToShort(this.txt_21.Text);
                    }
                }
                else if (this.drpStuff21.SelectedIndex == 0 && string.IsNullOrEmpty(this.txt_21.Text)) // Delete ration
                {
                    db.SchoolFoods.DeleteOnSubmit(schFood21);
                }
            }

            SupplySystem.SchoolFood schFood22 = (from sf in db.SchoolFoods
                                                 join cl in db.Calendars on sf.CalendarID equals cl.CalendarID
                                                 where sf.SchoolSubLevelID == schoolSubLevelId &&
                                                          cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 22
                                                 select sf).FirstOrDefault<SupplySystem.SchoolFood>();
            if (schFood22 == null) // Add mode
            {
                if (this.drpStuff22.SelectedIndex > 0 && !string.IsNullOrEmpty(this.txt_22.Text))
                {
                    db.SchoolFoods.InsertOnSubmit(new SupplySystem.SchoolFood
                    {
                        SchoolSubLevelID = schoolSubLevelId,
                        CycleFoodID = Public.ToShort(this.drpStuff22.SelectedValue),
                        CalendarID = db.Calendars.First<SupplySystem.Calendar>(cl => cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 22).CalendarID,
                        FoodCount = Public.ToShort(this.txt_22.Text),
                        SubmitDate = DateTime.Now
                    });
                }
            }
            else // Edit mode
            {
                if (this.drpStuff22.SelectedIndex > 0 && !string.IsNullOrEmpty(this.txt_22.Text))
                {
                    if (schFood22.CycleFoodID.ToString() != this.drpStuff22.SelectedValue) // Delete existing ration and add new one
                    {
                        db.SchoolFoods.DeleteOnSubmit(schFood22);
                        db.SchoolFoods.InsertOnSubmit(new SupplySystem.SchoolFood
                        {
                            SchoolSubLevelID = schoolSubLevelId,
                            CycleFoodID = Public.ToShort(this.drpStuff22.SelectedValue),
                            CalendarID = db.Calendars.First<SupplySystem.Calendar>(cl => cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 22).CalendarID,
                            FoodCount = Public.ToShort(this.txt_22.Text),
                            SubmitDate = DateTime.Now
                        });
                    }
                    else if (schFood22.FoodCount.ToString() != this.txt_22.Text.Trim()) // Edit FoodCount value
                    {
                        schFood22.FoodCount = Public.ToShort(this.txt_22.Text);
                    }
                }
                else if (this.drpStuff22.SelectedIndex == 0 && string.IsNullOrEmpty(this.txt_22.Text)) // Delete ration
                {
                    db.SchoolFoods.DeleteOnSubmit(schFood22);
                }
            }

            SupplySystem.SchoolFood schFood23 = (from sf in db.SchoolFoods
                                                 join cl in db.Calendars on sf.CalendarID equals cl.CalendarID
                                                 where sf.SchoolSubLevelID == schoolSubLevelId &&
                                                          cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 23
                                                 select sf).FirstOrDefault<SupplySystem.SchoolFood>();
            if (schFood23 == null) // Add mode
            {
                if (this.drpStuff23.SelectedIndex > 0 && !string.IsNullOrEmpty(this.txt_23.Text))
                {
                    db.SchoolFoods.InsertOnSubmit(new SupplySystem.SchoolFood
                    {
                        SchoolSubLevelID = schoolSubLevelId,
                        CycleFoodID = Public.ToShort(this.drpStuff23.SelectedValue),
                        CalendarID = db.Calendars.First<SupplySystem.Calendar>(cl => cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 23).CalendarID,
                        FoodCount = Public.ToShort(this.txt_23.Text),
                        SubmitDate = DateTime.Now
                    });
                }
            }
            else // Edit mode
            {
                if (this.drpStuff23.SelectedIndex > 0 && !string.IsNullOrEmpty(this.txt_23.Text))
                {
                    if (schFood23.CycleFoodID.ToString() != this.drpStuff23.SelectedValue) // Delete existing ration and add new one
                    {
                        db.SchoolFoods.DeleteOnSubmit(schFood23);
                        db.SchoolFoods.InsertOnSubmit(new SupplySystem.SchoolFood
                        {
                            SchoolSubLevelID = schoolSubLevelId,
                            CycleFoodID = Public.ToShort(this.drpStuff23.SelectedValue),
                            CalendarID = db.Calendars.First<SupplySystem.Calendar>(cl => cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 23).CalendarID,
                            FoodCount = Public.ToShort(this.txt_23.Text),
                            SubmitDate = DateTime.Now
                        });
                    }
                    else if (schFood23.FoodCount.ToString() != this.txt_23.Text.Trim()) // Edit FoodCount value
                    {
                        schFood23.FoodCount = Public.ToShort(this.txt_23.Text);
                    }
                }
                else if (this.drpStuff23.SelectedIndex == 0 && string.IsNullOrEmpty(this.txt_23.Text)) // Delete ration
                {
                    db.SchoolFoods.DeleteOnSubmit(schFood23);
                }
            }

            SupplySystem.SchoolFood schFood24 = (from sf in db.SchoolFoods
                                                 join cl in db.Calendars on sf.CalendarID equals cl.CalendarID
                                                 where sf.SchoolSubLevelID == schoolSubLevelId &&
                                                          cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 24
                                                 select sf).FirstOrDefault<SupplySystem.SchoolFood>();
            if (schFood24 == null) // Add mode
            {
                if (this.drpStuff24.SelectedIndex > 0 && !string.IsNullOrEmpty(this.txt_24.Text))
                {
                    db.SchoolFoods.InsertOnSubmit(new SupplySystem.SchoolFood
                    {
                        SchoolSubLevelID = schoolSubLevelId,
                        CycleFoodID = Public.ToShort(this.drpStuff24.SelectedValue),
                        CalendarID = db.Calendars.First<SupplySystem.Calendar>(cl => cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 24).CalendarID,
                        FoodCount = Public.ToShort(this.txt_24.Text),
                        SubmitDate = DateTime.Now
                    });
                }
            }
            else // Edit mode
            {
                if (this.drpStuff24.SelectedIndex > 0 && !string.IsNullOrEmpty(this.txt_24.Text))
                {
                    if (schFood24.CycleFoodID.ToString() != this.drpStuff24.SelectedValue) // Delete existing ration and add new one
                    {
                        db.SchoolFoods.DeleteOnSubmit(schFood24);
                        db.SchoolFoods.InsertOnSubmit(new SupplySystem.SchoolFood
                        {
                            SchoolSubLevelID = schoolSubLevelId,
                            CycleFoodID = Public.ToShort(this.drpStuff24.SelectedValue),
                            CalendarID = db.Calendars.First<SupplySystem.Calendar>(cl => cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 24).CalendarID,
                            FoodCount = Public.ToShort(this.txt_24.Text),
                            SubmitDate = DateTime.Now
                        });
                    }
                    else if (schFood24.FoodCount.ToString() != this.txt_24.Text.Trim()) // Edit FoodCount value
                    {
                        schFood24.FoodCount = Public.ToShort(this.txt_24.Text);
                    }
                }
                else if (this.drpStuff24.SelectedIndex == 0 && string.IsNullOrEmpty(this.txt_24.Text)) // Delete ration
                {
                    db.SchoolFoods.DeleteOnSubmit(schFood24);
                }
            }

            SupplySystem.SchoolFood schFood25 = (from sf in db.SchoolFoods
                                                 join cl in db.Calendars on sf.CalendarID equals cl.CalendarID
                                                 where sf.SchoolSubLevelID == schoolSubLevelId &&
                                                          cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 25
                                                 select sf).FirstOrDefault<SupplySystem.SchoolFood>();
            if (schFood25 == null) // Add mode
            {
                if (this.drpStuff25.SelectedIndex > 0 && !string.IsNullOrEmpty(this.txt_25.Text))
                {
                    db.SchoolFoods.InsertOnSubmit(new SupplySystem.SchoolFood
                    {
                        SchoolSubLevelID = schoolSubLevelId,
                        CycleFoodID = Public.ToShort(this.drpStuff25.SelectedValue),
                        CalendarID = db.Calendars.First<SupplySystem.Calendar>(cl => cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 25).CalendarID,
                        FoodCount = Public.ToShort(this.txt_25.Text),
                        SubmitDate = DateTime.Now
                    });
                }
            }
            else // Edit mode
            {
                if (this.drpStuff25.SelectedIndex > 0 && !string.IsNullOrEmpty(this.txt_25.Text))
                {
                    if (schFood25.CycleFoodID.ToString() != this.drpStuff25.SelectedValue) // Delete existing ration and add new one
                    {
                        db.SchoolFoods.DeleteOnSubmit(schFood25);
                        db.SchoolFoods.InsertOnSubmit(new SupplySystem.SchoolFood
                        {
                            SchoolSubLevelID = schoolSubLevelId,
                            CycleFoodID = Public.ToShort(this.drpStuff25.SelectedValue),
                            CalendarID = db.Calendars.First<SupplySystem.Calendar>(cl => cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 25).CalendarID,
                            FoodCount = Public.ToShort(this.txt_25.Text),
                            SubmitDate = DateTime.Now
                        });
                    }
                    else if (schFood25.FoodCount.ToString() != this.txt_25.Text.Trim()) // Edit FoodCount value
                    {
                        schFood25.FoodCount = Public.ToShort(this.txt_25.Text);
                    }
                }
                else if (this.drpStuff25.SelectedIndex == 0 && string.IsNullOrEmpty(this.txt_25.Text)) // Delete ration
                {
                    db.SchoolFoods.DeleteOnSubmit(schFood25);
                }
            }

            SupplySystem.SchoolFood schFood26 = (from sf in db.SchoolFoods
                                                 join cl in db.Calendars on sf.CalendarID equals cl.CalendarID
                                                 where sf.SchoolSubLevelID == schoolSubLevelId &&
                                                          cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 26
                                                 select sf).FirstOrDefault<SupplySystem.SchoolFood>();
            if (schFood26 == null) // Add mode
            {
                if (this.drpStuff26.SelectedIndex > 0 && !string.IsNullOrEmpty(this.txt_26.Text))
                {
                    db.SchoolFoods.InsertOnSubmit(new SupplySystem.SchoolFood
                    {
                        SchoolSubLevelID = schoolSubLevelId,
                        CycleFoodID = Public.ToShort(this.drpStuff26.SelectedValue),
                        CalendarID = db.Calendars.First<SupplySystem.Calendar>(cl => cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 26).CalendarID,
                        FoodCount = Public.ToShort(this.txt_26.Text),
                        SubmitDate = DateTime.Now
                    });
                }
            }
            else // Edit mode
            {
                if (this.drpStuff26.SelectedIndex > 0 && !string.IsNullOrEmpty(this.txt_26.Text))
                {
                    if (schFood26.CycleFoodID.ToString() != this.drpStuff26.SelectedValue) // Delete existing ration and add new one
                    {
                        db.SchoolFoods.DeleteOnSubmit(schFood26);
                        db.SchoolFoods.InsertOnSubmit(new SupplySystem.SchoolFood
                        {
                            SchoolSubLevelID = schoolSubLevelId,
                            CycleFoodID = Public.ToShort(this.drpStuff26.SelectedValue),
                            CalendarID = db.Calendars.First<SupplySystem.Calendar>(cl => cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 26).CalendarID,
                            FoodCount = Public.ToShort(this.txt_26.Text),
                            SubmitDate = DateTime.Now
                        });
                    }
                    else if (schFood26.FoodCount.ToString() != this.txt_26.Text.Trim()) // Edit FoodCount value
                    {
                        schFood26.FoodCount = Public.ToShort(this.txt_26.Text);
                    }
                }
                else if (this.drpStuff26.SelectedIndex == 0 && string.IsNullOrEmpty(this.txt_26.Text)) // Delete ration
                {
                    db.SchoolFoods.DeleteOnSubmit(schFood26);
                }
            }

            SupplySystem.SchoolFood schFood27 = (from sf in db.SchoolFoods
                                                 join cl in db.Calendars on sf.CalendarID equals cl.CalendarID
                                                 where sf.SchoolSubLevelID == schoolSubLevelId &&
                                                          cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 27
                                                 select sf).FirstOrDefault<SupplySystem.SchoolFood>();
            if (schFood27 == null) // Add mode
            {
                if (this.drpStuff27.SelectedIndex > 0 && !string.IsNullOrEmpty(this.txt_27.Text))
                {
                    db.SchoolFoods.InsertOnSubmit(new SupplySystem.SchoolFood
                    {
                        SchoolSubLevelID = schoolSubLevelId,
                        CycleFoodID = Public.ToShort(this.drpStuff27.SelectedValue),
                        CalendarID = db.Calendars.First<SupplySystem.Calendar>(cl => cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 27).CalendarID,
                        FoodCount = Public.ToShort(this.txt_27.Text),
                        SubmitDate = DateTime.Now
                    });
                }
            }
            else // Edit mode
            {
                if (this.drpStuff27.SelectedIndex > 0 && !string.IsNullOrEmpty(this.txt_27.Text))
                {
                    if (schFood27.CycleFoodID.ToString() != this.drpStuff27.SelectedValue) // Delete existing ration and add new one
                    {
                        db.SchoolFoods.DeleteOnSubmit(schFood27);
                        db.SchoolFoods.InsertOnSubmit(new SupplySystem.SchoolFood
                        {
                            SchoolSubLevelID = schoolSubLevelId,
                            CycleFoodID = Public.ToShort(this.drpStuff27.SelectedValue),
                            CalendarID = db.Calendars.First<SupplySystem.Calendar>(cl => cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 27).CalendarID,
                            FoodCount = Public.ToShort(this.txt_27.Text),
                            SubmitDate = DateTime.Now
                        });
                    }
                    else if (schFood27.FoodCount.ToString() != this.txt_27.Text.Trim()) // Edit FoodCount value
                    {
                        schFood27.FoodCount = Public.ToShort(this.txt_27.Text);
                    }
                }
                else if (this.drpStuff27.SelectedIndex == 0 && string.IsNullOrEmpty(this.txt_27.Text)) // Delete ration
                {
                    db.SchoolFoods.DeleteOnSubmit(schFood27);
                }
            }

            SupplySystem.SchoolFood schFood28 = (from sf in db.SchoolFoods
                                                 join cl in db.Calendars on sf.CalendarID equals cl.CalendarID
                                                 where sf.SchoolSubLevelID == schoolSubLevelId &&
                                                          cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 28
                                                 select sf).FirstOrDefault<SupplySystem.SchoolFood>();
            if (schFood28 == null) // Add mode
            {
                if (this.drpStuff28.SelectedIndex > 0 && !string.IsNullOrEmpty(this.txt_28.Text))
                {
                    db.SchoolFoods.InsertOnSubmit(new SupplySystem.SchoolFood
                    {
                        SchoolSubLevelID = schoolSubLevelId,
                        CycleFoodID = Public.ToShort(this.drpStuff28.SelectedValue),
                        CalendarID = db.Calendars.First<SupplySystem.Calendar>(cl => cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 28).CalendarID,
                        FoodCount = Public.ToShort(this.txt_28.Text),
                        SubmitDate = DateTime.Now
                    });
                }
            }
            else // Edit mode
            {
                if (this.drpStuff28.SelectedIndex > 0 && !string.IsNullOrEmpty(this.txt_28.Text))
                {
                    if (schFood28.CycleFoodID.ToString() != this.drpStuff28.SelectedValue) // Delete existing ration and add new one
                    {
                        db.SchoolFoods.DeleteOnSubmit(schFood28);
                        db.SchoolFoods.InsertOnSubmit(new SupplySystem.SchoolFood
                        {
                            SchoolSubLevelID = schoolSubLevelId,
                            CycleFoodID = Public.ToShort(this.drpStuff28.SelectedValue),
                            CalendarID = db.Calendars.First<SupplySystem.Calendar>(cl => cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 28).CalendarID,
                            FoodCount = Public.ToShort(this.txt_28.Text),
                            SubmitDate = DateTime.Now
                        });
                    }
                    else if (schFood28.FoodCount.ToString() != this.txt_28.Text.Trim()) // Edit FoodCount value
                    {
                        schFood28.FoodCount = Public.ToShort(this.txt_28.Text);
                    }
                }
                else if (this.drpStuff28.SelectedIndex == 0 && string.IsNullOrEmpty(this.txt_28.Text)) // Delete ration
                {
                    db.SchoolFoods.DeleteOnSubmit(schFood28);
                }
            }

            SupplySystem.SchoolFood schFood29 = (from sf in db.SchoolFoods
                                                 join cl in db.Calendars on sf.CalendarID equals cl.CalendarID
                                                 where sf.SchoolSubLevelID == schoolSubLevelId &&
                                                          cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 29
                                                 select sf).FirstOrDefault<SupplySystem.SchoolFood>();
            if (schFood29 == null) // Add mode
            {
                if (this.drpStuff29.SelectedIndex > 0 && !string.IsNullOrEmpty(this.txt_29.Text))
                {
                    db.SchoolFoods.InsertOnSubmit(new SupplySystem.SchoolFood
                    {
                        SchoolSubLevelID = schoolSubLevelId,
                        CycleFoodID = Public.ToShort(this.drpStuff29.SelectedValue),
                        CalendarID = db.Calendars.First<SupplySystem.Calendar>(cl => cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 29).CalendarID,
                        FoodCount = Public.ToShort(this.txt_29.Text),
                        SubmitDate = DateTime.Now
                    });
                }
            }
            else // Edit mode
            {
                if (this.drpStuff29.SelectedIndex > 0 && !string.IsNullOrEmpty(this.txt_29.Text))
                {
                    if (schFood29.CycleFoodID.ToString() != this.drpStuff29.SelectedValue) // Delete existing ration and add new one
                    {
                        db.SchoolFoods.DeleteOnSubmit(schFood29);
                        db.SchoolFoods.InsertOnSubmit(new SupplySystem.SchoolFood
                        {
                            SchoolSubLevelID = schoolSubLevelId,
                            CycleFoodID = Public.ToShort(this.drpStuff29.SelectedValue),
                            CalendarID = db.Calendars.First<SupplySystem.Calendar>(cl => cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 29).CalendarID,
                            FoodCount = Public.ToShort(this.txt_29.Text),
                            SubmitDate = DateTime.Now
                        });
                    }
                    else if (schFood29.FoodCount.ToString() != this.txt_29.Text.Trim()) // Edit FoodCount value
                    {
                        schFood29.FoodCount = Public.ToShort(this.txt_29.Text);
                    }
                }
                else if (this.drpStuff29.SelectedIndex == 0 && string.IsNullOrEmpty(this.txt_29.Text)) // Delete ration
                {
                    db.SchoolFoods.DeleteOnSubmit(schFood29);
                }
            }

            SupplySystem.SchoolFood schFood30 = (from sf in db.SchoolFoods
                                                 join cl in db.Calendars on sf.CalendarID equals cl.CalendarID
                                                 where sf.SchoolSubLevelID == schoolSubLevelId &&
                                                          cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 30
                                                 select sf).FirstOrDefault<SupplySystem.SchoolFood>();
            if (schFood30 == null) // Add mode
            {
                if (this.drpStuff30.SelectedIndex > 0 && !string.IsNullOrEmpty(this.txt_30.Text))
                {
                    db.SchoolFoods.InsertOnSubmit(new SupplySystem.SchoolFood
                    {
                        SchoolSubLevelID = schoolSubLevelId,
                        CycleFoodID = Public.ToShort(this.drpStuff30.SelectedValue),
                        CalendarID = db.Calendars.First<SupplySystem.Calendar>(cl => cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 30).CalendarID,
                        FoodCount = Public.ToShort(this.txt_30.Text),
                        SubmitDate = DateTime.Now
                    });
                }
            }
            else // Edit mode
            {
                if (this.drpStuff30.SelectedIndex > 0 && !string.IsNullOrEmpty(this.txt_30.Text))
                {
                    if (schFood30.CycleFoodID.ToString() != this.drpStuff30.SelectedValue) // Delete existing ration and add new one
                    {
                        db.SchoolFoods.DeleteOnSubmit(schFood30);
                        db.SchoolFoods.InsertOnSubmit(new SupplySystem.SchoolFood
                        {
                            SchoolSubLevelID = schoolSubLevelId,
                            CycleFoodID = Public.ToShort(this.drpStuff30.SelectedValue),
                            CalendarID = db.Calendars.First<SupplySystem.Calendar>(cl => cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 30).CalendarID,
                            FoodCount = Public.ToShort(this.txt_30.Text),
                            SubmitDate = DateTime.Now
                        });
                    }
                    else if (schFood30.FoodCount.ToString() != this.txt_30.Text.Trim()) // Edit FoodCount value
                    {
                        schFood30.FoodCount = Public.ToShort(this.txt_30.Text);
                    }
                }
                else if (this.drpStuff30.SelectedIndex == 0 && string.IsNullOrEmpty(this.txt_30.Text)) // Delete ration
                {
                    db.SchoolFoods.DeleteOnSubmit(schFood30);
                }
            }

            SupplySystem.SchoolFood schFood31 = (from sf in db.SchoolFoods
                                                 join cl in db.Calendars on sf.CalendarID equals cl.CalendarID
                                                 where sf.SchoolSubLevelID == schoolSubLevelId &&
                                                          cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 31
                                                 select sf).FirstOrDefault<SupplySystem.SchoolFood>();
            if (schFood31 == null) // Add mode
            {
                if (this.drpStuff31.SelectedIndex > 0 && !string.IsNullOrEmpty(this.txt_31.Text))
                {
                    db.SchoolFoods.InsertOnSubmit(new SupplySystem.SchoolFood
                    {
                        SchoolSubLevelID = schoolSubLevelId,
                        CycleFoodID = Public.ToShort(this.drpStuff31.SelectedValue),
                        CalendarID = db.Calendars.First<SupplySystem.Calendar>(cl => cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 31).CalendarID,
                        FoodCount = Public.ToShort(this.txt_31.Text),
                        SubmitDate = DateTime.Now
                    });
                }
            }
            else // Edit mode
            {
                if (this.drpStuff31.SelectedIndex > 0 && !string.IsNullOrEmpty(this.txt_31.Text))
                {
                    if (schFood31.CycleFoodID.ToString() != this.drpStuff31.SelectedValue) // Delete existing ration and add new one
                    {
                        db.SchoolFoods.DeleteOnSubmit(schFood31);
                        db.SchoolFoods.InsertOnSubmit(new SupplySystem.SchoolFood
                        {
                            SchoolSubLevelID = schoolSubLevelId,
                            CycleFoodID = Public.ToShort(this.drpStuff31.SelectedValue),
                            CalendarID = db.Calendars.First<SupplySystem.Calendar>(cl => cl.SolarYear == solarYear && cl.SolarMonth == solarMonth && cl.SolarDay == 31).CalendarID,
                            FoodCount = Public.ToShort(this.txt_31.Text),
                            SubmitDate = DateTime.Now
                        });
                    }
                    else if (schFood31.FoodCount.ToString() != this.txt_31.Text.Trim()) // Edit FoodCount value
                    {
                        schFood31.FoodCount = Public.ToShort(this.txt_31.Text);
                    }
                }
                else if (this.drpStuff31.SelectedIndex == 0 && string.IsNullOrEmpty(this.txt_31.Text)) // Delete ration
                {
                    db.SchoolFoods.DeleteOnSubmit(schFood31);
                }
            }

            #endregion

            db.SubmitChanges();
            this.lblMessage.Text = Public.SUCCESSMESSAGE;
        }
    }

    private ArrayList GetFoods()
    {
        ArrayList arrayList = HttpContext.Current.Cache["Foods"] as ArrayList;
        if (arrayList == null)
        {
            arrayList = new ArrayList();
            var foods = from cf in db.CycleFoods
                        join st in db.REP_Stuffs on cf.StuffID equals st.StuffID
                        where cf.CycleID == Public.ActiveCycle.CycleID && cf.Available && !cf.IsDaily
                        select new { cf.CycleFoodID, st.StuffName };
            arrayList.Add(new { CycleFoodID = 0, StuffName = "----" });

            foreach (var stuff in foods)
            {
                arrayList.Add(new { stuff.CycleFoodID, stuff.StuffName });
            }
            HttpContext.Current.Cache.Insert("Foods", arrayList, null, DateTime.MaxValue, TimeSpan.FromMinutes(6));
        }
        return arrayList;
    }

    private void LoadFoods()
    {
        if (this.drpStuff1.Items.Count == 0)
        {
            ArrayList foodList = GetFoods();
            this.drpStuff1.DataSource = foodList;
            this.drpStuff2.DataSource = foodList;
            this.drpStuff3.DataSource = foodList;
            this.drpStuff4.DataSource = foodList;
            this.drpStuff5.DataSource = foodList;
            this.drpStuff6.DataSource = foodList;
            this.drpStuff7.DataSource = foodList;
            this.drpStuff8.DataSource = foodList;
            this.drpStuff9.DataSource = foodList;
            this.drpStuff10.DataSource = foodList;
            this.drpStuff11.DataSource = foodList;
            this.drpStuff12.DataSource = foodList;
            this.drpStuff13.DataSource = foodList;
            this.drpStuff14.DataSource = foodList;
            this.drpStuff15.DataSource = foodList;
            this.drpStuff16.DataSource = foodList;
            this.drpStuff16.DataSource = foodList;
            this.drpStuff17.DataSource = foodList;
            this.drpStuff18.DataSource = foodList;
            this.drpStuff19.DataSource = foodList;
            this.drpStuff20.DataSource = foodList;
            this.drpStuff21.DataSource = foodList;
            this.drpStuff22.DataSource = foodList;
            this.drpStuff23.DataSource = foodList;
            this.drpStuff24.DataSource = foodList;
            this.drpStuff25.DataSource = foodList;
            this.drpStuff26.DataSource = foodList;
            this.drpStuff27.DataSource = foodList;
            this.drpStuff28.DataSource = foodList;
            this.drpStuff29.DataSource = foodList;
            this.drpStuff30.DataSource = foodList;
            this.drpStuff31.DataSource = foodList;
            this.Page.DataBind();
        }
    }

    private void SetHolidays(short solarYear, byte solarMonth)
    {
        List<SupplySystem.Calendar> calendarList = db.Calendars.Where<SupplySystem.Calendar>(c => c.SolarYear == solarYear &&
                                                                                                                                            c.SolarMonth == solarMonth).ToList<SupplySystem.Calendar>();
        if (calendarList.Count > 0)
        {
            this.drpStuff30.Enabled = false;
            this.drpStuff30.SelectedIndex = 0;
            this.td_30.Style["background-color"] = RED;
            this.txt_30.Enabled = false;
            this.txt_30.Text = null;
            this.drpStuff31.Enabled = false;
            this.drpStuff31.SelectedIndex = 0;
            this.td_31.Style["background-color"] = RED;
            this.txt_31.Enabled = false;
            this.txt_31.Text = null;

            for (int i = 0; i < calendarList.Count; i++)
            {
                switch (calendarList[i].SolarDay)
                {
                    case 1:
                        if (!calendarList[i].IsHoliday)
                        {
                            this.drpStuff1.Enabled = true;
                            this.td_1.Style["background-color"] = YELLOW;
                            this.txt_1.Enabled = true;
                        }
                        break;

                    case 2:
                        if (!calendarList[i].IsHoliday)
                        {
                            this.drpStuff2.Enabled = true;
                            this.td_2.Style["background-color"] = YELLOW;
                            this.txt_2.Enabled = true;
                        }
                        break;

                    case 3:
                        if (!calendarList[i].IsHoliday)
                        {
                            this.drpStuff3.Enabled = true;
                            this.td_3.Style["background-color"] = YELLOW;
                            this.txt_3.Enabled = true;
                        }
                        break;

                    case 4:
                        if (!calendarList[i].IsHoliday)
                        {
                            this.drpStuff4.Enabled = true;
                            this.td_4.Style["background-color"] = YELLOW;
                            this.txt_4.Enabled = true;
                        }
                        break;

                    case 5:
                        if (!calendarList[i].IsHoliday)
                        {
                            this.drpStuff5.Enabled = true;
                            this.td_5.Style["background-color"] = YELLOW;
                            this.txt_5.Enabled = true;
                        }
                        break;

                    case 6:
                        if (!calendarList[i].IsHoliday)
                        {
                            this.drpStuff6.Enabled = true;
                            this.td_6.Style["background-color"] = YELLOW;
                            this.txt_6.Enabled = true;
                        }
                        break;

                    case 7:
                        if (!calendarList[i].IsHoliday)
                        {
                            this.drpStuff7.Enabled = true;
                            this.td_7.Style["background-color"] = YELLOW;
                            this.txt_7.Enabled = true;
                        }
                        break;

                    case 8:
                        if (!calendarList[i].IsHoliday)
                        {
                            this.drpStuff8.Enabled = true;
                            this.td_8.Style["background-color"] = YELLOW;
                            this.txt_8.Enabled = true;
                        }
                        break;

                    case 9:
                        if (!calendarList[i].IsHoliday)
                        {
                            this.drpStuff9.Enabled = true;
                            this.td_9.Style["background-color"] = YELLOW;
                            this.txt_9.Enabled = true;
                        }
                        break;

                    case 10:
                        if (!calendarList[i].IsHoliday)
                        {
                            this.drpStuff10.Enabled = true;
                            this.td_10.Style["background-color"] = YELLOW;
                            this.txt_10.Enabled = true;
                        }
                        break;

                    case 11:
                        if (!calendarList[i].IsHoliday)
                        {
                            this.drpStuff11.Enabled = true;
                            this.td_11.Style["background-color"] = YELLOW;
                            this.txt_11.Enabled = true;
                        }
                        break;

                    case 12:
                        if (!calendarList[i].IsHoliday)
                        {
                            this.drpStuff12.Enabled = true;
                            this.td_12.Style["background-color"] = YELLOW;
                            this.txt_12.Enabled = true;
                        }
                        break;

                    case 13:
                        if (!calendarList[i].IsHoliday)
                        {
                            this.drpStuff13.Enabled = true;
                            this.td_13.Style["background-color"] = YELLOW;
                            this.txt_13.Enabled = true;
                        }
                        break;

                    case 14:
                        if (!calendarList[i].IsHoliday)
                        {
                            this.drpStuff14.Enabled = true;
                            this.td_14.Style["background-color"] = YELLOW;
                            this.txt_14.Enabled = true;
                        }
                        break;

                    case 15:
                        if (!calendarList[i].IsHoliday)
                        {
                            this.drpStuff15.Enabled = true;
                            this.td_15.Style["background-color"] = YELLOW;
                            this.txt_15.Enabled = true;
                        }
                        break;

                    case 16:
                        if (!calendarList[i].IsHoliday)
                        {
                            this.drpStuff16.Enabled = true;
                            this.td_16.Style["background-color"] = YELLOW;
                            this.txt_16.Enabled = true;
                        }
                        break;

                    case 17:
                        if (!calendarList[i].IsHoliday)
                        {
                            this.drpStuff17.Enabled = true;
                            this.td_17.Style["background-color"] = YELLOW;
                            this.txt_17.Enabled = true;
                        }
                        break;

                    case 18:
                        if (!calendarList[i].IsHoliday)
                        {
                            this.drpStuff18.Enabled = true;
                            this.td_18.Style["background-color"] = YELLOW;
                            this.txt_18.Enabled = true;
                        }
                        break;

                    case 19:
                        if (!calendarList[i].IsHoliday)
                        {
                            this.drpStuff19.Enabled = true;
                            this.td_19.Style["background-color"] = YELLOW;
                            this.txt_19.Enabled = true;
                        }
                        break;

                    case 20:
                        if (!calendarList[i].IsHoliday)
                        {
                            this.drpStuff20.Enabled = true;
                            this.td_20.Style["background-color"] = YELLOW;
                            this.txt_20.Enabled = true;
                        }
                        break;

                    case 21:
                        if (!calendarList[i].IsHoliday)
                        {
                            this.drpStuff21.Enabled = true;
                            this.td_21.Style["background-color"] = YELLOW;
                            this.txt_21.Enabled = true;
                        }
                        break;

                    case 22:
                        if (!calendarList[i].IsHoliday)
                        {
                            this.drpStuff22.Enabled = true;
                            this.td_22.Style["background-color"] = YELLOW;
                            this.txt_22.Enabled = true;
                        }
                        break;

                    case 23:
                        if (!calendarList[i].IsHoliday)
                        {
                            this.drpStuff23.Enabled = true;
                            this.td_23.Style["background-color"] = YELLOW;
                            this.txt_23.Enabled = true;
                        }
                        break;

                    case 24:
                        if (!calendarList[i].IsHoliday)
                        {
                            this.drpStuff24.Enabled = true;
                            this.td_24.Style["background-color"] = YELLOW;
                            this.txt_24.Enabled = true;
                        }
                        break;

                    case 25:
                        if (!calendarList[i].IsHoliday)
                        {
                            this.drpStuff25.Enabled = true;
                            this.td_25.Style["background-color"] = YELLOW;
                            this.txt_25.Enabled = true;
                        }
                        break;

                    case 26:
                        if (!calendarList[i].IsHoliday)
                        {
                            this.drpStuff26.Enabled = true;
                            this.td_26.Style["background-color"] = YELLOW;
                            this.txt_26.Enabled = true;
                        }
                        break;

                    case 27:
                        if (!calendarList[i].IsHoliday)
                        {
                            this.drpStuff27.Enabled = true;
                            this.td_27.Style["background-color"] = YELLOW;
                            this.txt_27.Enabled = true;
                        }
                        break;

                    case 28:
                        if (!calendarList[i].IsHoliday)
                        {
                            this.drpStuff28.Enabled = true;
                            this.td_28.Style["background-color"] = YELLOW;
                            this.txt_28.Enabled = true;
                        }
                        break;

                    case 29:
                        if (!calendarList[i].IsHoliday)
                        {
                            this.drpStuff29.Enabled = true;
                            this.td_29.Style["background-color"] = YELLOW;
                            this.txt_29.Enabled = true;
                        }
                        break;

                    case 30:
                        if (!calendarList[i].IsHoliday)
                        {
                            this.drpStuff30.Enabled = true;
                            this.td_30.Style["background-color"] = YELLOW;
                            this.txt_30.Enabled = true;
                        }
                        break;

                    case 31:
                        if (!calendarList[i].IsHoliday)
                        {
                            this.drpStuff31.Enabled = true;
                            this.td_31.Style["background-color"] = YELLOW;
                            this.txt_31.Enabled = true;
                        }
                        break;
                }
            }
        }
    }

    private bool FillHeader()
    {
        bool result = false;
        var school = from s in db.Schools
                     join ar in db.Areas on s.AreaCode equals ar.AreaCode
                     join slv in db.SchoolLevels on s.SchoolID equals slv.SchoolID
                     join lv in db.Levels on slv.LevelID equals lv.LevelID
                     where s.SchoolCode == Public.ToInt(this.txtSchoolCode.Text) && slv.LockOutDate == null
                     select
                     new
                     {
                         s.SchoolID,
                         s.SchoolCode,
                         s.SchoolName,
                         s.Gender,
                         lv.LevelName,
                         ar.AreaCode,
                         EmployeesCount = s.EmployeesCount_Fixed.GetValueOrDefault() + s.EmployeesCount_Changable.GetValueOrDefault()
                     };

        if (System.Web.HttpContext.Current.User.IsInRole("AreaManager"))
        {
            school = from q in school
                     where q.AreaCode == (from u in db.Users join ur in db.UsersInRoles on u.UserID equals ur.UserID where u.UserName.Equals(HttpContext.Current.User.Identity.Name) select ur.AreaCode).SingleOrDefault()
                     select q;
        }
        else if (System.Web.HttpContext.Current.User.IsInRole("SchoolManager"))
        {
            school = from q in school
                     where q.SchoolCode == Public.ToInt(HttpContext.Current.User.Identity.Name)
                     select q;
        }

        int employeesCount = 0;
        foreach (var item in school)
        {
            this.hdnSchoolId.Value = item.SchoolID.ToString();
            this.txtSchoolName.Text = item.SchoolName;
            this.lblLevel.Text = item.LevelName;
            this.lblGender.Text = Public.GetGender(item.Gender);
            employeesCount = item.EmployeesCount;

            var subLevels = from slv in db.SchoolLevels
                            join ssl in db.SchoolSubLevels on slv.SchoolLevelID equals ssl.SchoolLevelID
                            join sl in db.SubLevels on ssl.SubLevelID equals sl.SubLevelID
                            where slv.SchoolID == Public.ToInt(this.hdnSchoolId.Value) && slv.LockOutDate == null
                            select
                            new
                            {
                                ssl.SchoolSubLevelID,
                                sl.SubLevelName,
                                StudentCount = ssl.BoysCount.GetValueOrDefault() + ssl.GirlsCount.GetValueOrDefault()
                            };

            this.drpSublevel.Items.Clear();
            foreach (var sl in subLevels)
            {
                if (sl.SubLevelName.Equals("Employees"))
                {
                    this.drpSublevel.Items.Add(new ListItem("پرسنل", string.Concat(sl.SchoolSubLevelID, "|", employeesCount)));
                }
                else
                {
                    this.drpSublevel.Items.Add(new ListItem(sl.SubLevelName, string.Concat(sl.SchoolSubLevelID, "|", sl.StudentCount)));
                }
            }
            this.lblStudentCount.Text = this.drpSublevel.SelectedValue.Split('|')[1];
            result = true;
        }

        return result;
    }

    private void ResetHeader()
    {
        this.hdnSchoolId.Value = null;
        this.drpMonths.SelectedIndex = 0;
        this.drpMonths.Enabled = false;
        this.drpSublevel.Items.Clear();
        this.drpSublevel.Enabled = false;
        this.lblLevel.Text = null;
        this.lblGender.Text = null;
        this.txtSchoolName.Text = null;
        this.lblStudentCount.Text = null;
    }

    private void ResetDays()
    {
        this.drpStuff1.Enabled = false;
        this.drpStuff1.SelectedIndex = 0;
        this.td_1.Style["background-color"] = RED;
        this.txt_1.Enabled = false;
        this.txt_1.Text = null;
        this.drpStuff2.Enabled = false;
        this.drpStuff2.SelectedIndex = 0;
        this.td_2.Style["background-color"] = RED;
        this.txt_2.Enabled = false;
        this.txt_2.Text = null;
        this.drpStuff3.Enabled = false;
        this.drpStuff3.SelectedIndex = 0;
        this.td_3.Style["background-color"] = RED;
        this.txt_3.Enabled = false;
        this.txt_3.Text = null;
        this.drpStuff4.Enabled = false;
        this.drpStuff4.SelectedIndex = 0;
        this.td_4.Style["background-color"] = RED;
        this.txt_4.Enabled = false;
        this.txt_4.Text = null;
        this.drpStuff5.Enabled = false;
        this.drpStuff5.SelectedIndex = 0;
        this.td_5.Style["background-color"] = RED;
        this.txt_5.Enabled = false;
        this.txt_5.Text = null;
        this.drpStuff6.Enabled = false;
        this.drpStuff6.SelectedIndex = 0;
        this.td_6.Style["background-color"] = RED;
        this.txt_6.Enabled = false;
        this.txt_6.Text = null;
        this.drpStuff7.Enabled = false;
        this.drpStuff7.SelectedIndex = 0;
        this.td_7.Style["background-color"] = RED;
        this.txt_7.Enabled = false;
        this.txt_7.Text = null;
        this.drpStuff8.Enabled = false;
        this.drpStuff8.SelectedIndex = 0;
        this.td_8.Style["background-color"] = RED;
        this.txt_8.Enabled = false;
        this.txt_8.Text = null;
        this.drpStuff9.Enabled = false;
        this.drpStuff9.SelectedIndex = 0;
        this.td_9.Style["background-color"] = RED;
        this.txt_9.Enabled = false;
        this.txt_9.Text = null;
        this.drpStuff10.Enabled = false;
        this.drpStuff10.SelectedIndex = 0;
        this.td_10.Style["background-color"] = RED;
        this.txt_10.Enabled = false;
        this.txt_10.Text = null;
        this.drpStuff11.Enabled = false;
        this.drpStuff11.SelectedIndex = 0;
        this.td_11.Style["background-color"] = RED;
        this.txt_11.Enabled = false;
        this.txt_11.Text = null;
        this.drpStuff12.Enabled = false;
        this.drpStuff12.SelectedIndex = 0;
        this.td_12.Style["background-color"] = RED;
        this.txt_12.Enabled = false;
        this.txt_12.Text = null;
        this.drpStuff13.Enabled = false;
        this.drpStuff13.SelectedIndex = 0;
        this.td_13.Style["background-color"] = RED;
        this.txt_13.Enabled = false;
        this.txt_13.Text = null;
        this.drpStuff14.Enabled = false;
        this.drpStuff14.SelectedIndex = 0;
        this.td_14.Style["background-color"] = RED;
        this.txt_14.Enabled = false;
        this.txt_14.Text = null;
        this.drpStuff15.Enabled = false;
        this.drpStuff15.SelectedIndex = 0;
        this.td_15.Style["background-color"] = RED;
        this.txt_15.Enabled = false;
        this.txt_15.Text = null;
        this.drpStuff16.Enabled = false;
        this.drpStuff16.SelectedIndex = 0;
        this.td_16.Style["background-color"] = RED;
        this.txt_16.Enabled = false;
        this.txt_16.Text = null;
        this.drpStuff17.Enabled = false;
        this.drpStuff17.SelectedIndex = 0;
        this.td_17.Style["background-color"] = RED;
        this.txt_17.Enabled = false;
        this.txt_17.Text = null;
        this.drpStuff18.Enabled = false;
        this.drpStuff18.SelectedIndex = 0;
        this.td_18.Style["background-color"] = RED;
        this.txt_18.Enabled = false;
        this.txt_18.Text = null;
        this.drpStuff19.Enabled = false;
        this.drpStuff19.SelectedIndex = 0;
        this.td_19.Style["background-color"] = RED;
        this.txt_19.Enabled = false;
        this.txt_19.Text = null;
        this.drpStuff20.Enabled = false;
        this.drpStuff20.SelectedIndex = 0;
        this.td_20.Style["background-color"] = RED;
        this.txt_20.Enabled = false;
        this.txt_20.Text = null;
        this.drpStuff21.Enabled = false;
        this.drpStuff21.SelectedIndex = 0;
        this.td_21.Style["background-color"] = RED;
        this.txt_21.Enabled = false;
        this.txt_21.Text = null;
        this.drpStuff22.Enabled = false;
        this.drpStuff22.SelectedIndex = 0;
        this.td_22.Style["background-color"] = RED;
        this.txt_22.Enabled = false;
        this.txt_22.Text = null;
        this.drpStuff23.Enabled = false;
        this.drpStuff23.SelectedIndex = 0;
        this.td_23.Style["background-color"] = RED;
        this.txt_23.Enabled = false;
        this.txt_23.Text = null;
        this.drpStuff24.Enabled = false;
        this.drpStuff24.SelectedIndex = 0;
        this.td_24.Style["background-color"] = RED;
        this.txt_24.Enabled = false;
        this.txt_24.Text = null;
        this.drpStuff25.Enabled = false;
        this.drpStuff25.SelectedIndex = 0;
        this.td_25.Style["background-color"] = RED;
        this.txt_25.Enabled = false;
        this.txt_25.Text = null;
        this.drpStuff26.Enabled = false;
        this.drpStuff26.SelectedIndex = 0;
        this.td_26.Style["background-color"] = RED;
        this.txt_26.Enabled = false;
        this.txt_26.Text = null;
        this.drpStuff27.Enabled = false;
        this.drpStuff27.SelectedIndex = 0;
        this.td_27.Style["background-color"] = RED;
        this.txt_27.Enabled = false;
        this.txt_27.Text = null;
        this.drpStuff28.Enabled = false;
        this.drpStuff28.SelectedIndex = 0;
        this.td_28.Style["background-color"] = RED;
        this.txt_28.Enabled = false;
        this.txt_28.Text = null;
        this.drpStuff29.Enabled = false;
        this.drpStuff29.SelectedIndex = 0;
        this.td_29.Style["background-color"] = RED;
        this.txt_29.Enabled = false;
        this.txt_29.Text = null;
        this.drpStuff30.Enabled = false;
        this.drpStuff30.SelectedIndex = 0;
        this.td_30.Style["background-color"] = RED;
        this.txt_30.Enabled = false;
        this.txt_30.Text = null;
        this.drpStuff31.Enabled = false;
        this.drpStuff31.SelectedIndex = 0;
        this.td_31.Style["background-color"] = RED;
        this.txt_31.Enabled = false;
        this.txt_31.Text = null;
    }

    private void ResetFoods()
    {
        if (this.drpStuff1.Items.Count > 0)
        {
            this.drpStuff1.SelectedIndex = 0;
            this.drpStuff2.SelectedIndex = 0;
            this.drpStuff3.SelectedIndex = 0;
            this.drpStuff4.SelectedIndex = 0;
            this.drpStuff5.SelectedIndex = 0;
            this.drpStuff6.SelectedIndex = 0;
            this.drpStuff7.SelectedIndex = 0;
            this.drpStuff8.SelectedIndex = 0;
            this.drpStuff9.SelectedIndex = 0;
            this.drpStuff10.SelectedIndex = 0;
            this.drpStuff11.SelectedIndex = 0;
            this.drpStuff12.SelectedIndex = 0;
            this.drpStuff13.SelectedIndex = 0;
            this.drpStuff14.SelectedIndex = 0;
            this.drpStuff15.SelectedIndex = 0;
            this.drpStuff16.SelectedIndex = 0;
            this.drpStuff17.SelectedIndex = 0;
            this.drpStuff18.SelectedIndex = 0;
            this.drpStuff19.SelectedIndex = 0;
            this.drpStuff20.SelectedIndex = 0;
            this.drpStuff21.SelectedIndex = 0;
            this.drpStuff22.SelectedIndex = 0;
            this.drpStuff23.SelectedIndex = 0;
            this.drpStuff24.SelectedIndex = 0;
            this.drpStuff25.SelectedIndex = 0;
            this.drpStuff26.SelectedIndex = 0;
            this.drpStuff27.SelectedIndex = 0;
            this.drpStuff28.SelectedIndex = 0;
            this.drpStuff29.SelectedIndex = 0;
            this.drpStuff30.SelectedIndex = 0;
            this.drpStuff31.SelectedIndex = 0;
        }
    }

    private void ResetRations()
    {
        this.txt_1.Text = null;
        this.txt_2.Text = null;
        this.txt_3.Text = null;
        this.txt_4.Text = null;
        this.txt_5.Text = null;
        this.txt_6.Text = null;
        this.txt_7.Text = null;
        this.txt_8.Text = null;
        this.txt_9.Text = null;
        this.txt_10.Text = null;
        this.txt_11.Text = null;
        this.txt_12.Text = null;
        this.txt_13.Text = null;
        this.txt_14.Text = null;
        this.txt_15.Text = null;
        this.txt_16.Text = null;
        this.txt_17.Text = null;
        this.txt_18.Text = null;
        this.txt_19.Text = null;
        this.txt_20.Text = null;
        this.txt_21.Text = null;
        this.txt_22.Text = null;
        this.txt_23.Text = null;
        this.txt_24.Text = null;
        this.txt_25.Text = null;
        this.txt_26.Text = null;
        this.txt_27.Text = null;
        this.txt_28.Text = null;
        this.txt_29.Text = null;
        this.txt_30.Text = null;
        this.txt_31.Text = null;
        this.txtSum.Text = null;
    }

    private void SetDayRation(byte dayIndex, int cycleFoodId, short ration)
    {
        switch (dayIndex)
        {
            case 1:
                this.drpStuff1.SelectedValue = cycleFoodId.ToString();
                this.txt_1.Text = ration.ToString();
                break;

            case 2:
                this.drpStuff2.SelectedValue = cycleFoodId.ToString();
                this.txt_2.Text = ration.ToString();
                break;

            case 3:
                this.drpStuff3.SelectedValue = cycleFoodId.ToString();
                this.txt_3.Text = ration.ToString();
                break;

            case 4:
                this.drpStuff4.SelectedValue = cycleFoodId.ToString();
                this.txt_4.Text = ration.ToString();
                break;

            case 5:
                this.drpStuff5.SelectedValue = cycleFoodId.ToString();
                this.txt_5.Text = ration.ToString();
                break;

            case 6:
                this.drpStuff6.SelectedValue = cycleFoodId.ToString();
                this.txt_6.Text = ration.ToString();
                break;

            case 7:
                this.drpStuff7.SelectedValue = cycleFoodId.ToString();
                this.txt_7.Text = ration.ToString();
                break;

            case 8:
                this.drpStuff8.SelectedValue = cycleFoodId.ToString();
                this.txt_8.Text = ration.ToString();
                break;

            case 9:
                this.drpStuff9.SelectedValue = cycleFoodId.ToString();
                this.txt_9.Text = ration.ToString();
                break;

            case 10:
                this.drpStuff10.SelectedValue = cycleFoodId.ToString();
                this.txt_10.Text = ration.ToString();
                break;

            case 11:
                this.drpStuff11.SelectedValue = cycleFoodId.ToString();
                this.txt_11.Text = ration.ToString();
                break;

            case 12:
                this.drpStuff12.SelectedValue = cycleFoodId.ToString();
                this.txt_12.Text = ration.ToString();
                break;

            case 13:
                this.drpStuff13.SelectedValue = cycleFoodId.ToString();
                this.txt_13.Text = ration.ToString();
                break;

            case 14:
                this.drpStuff14.SelectedValue = cycleFoodId.ToString();
                this.txt_14.Text = ration.ToString();
                break;

            case 15:
                this.drpStuff15.SelectedValue = cycleFoodId.ToString();
                this.txt_15.Text = ration.ToString();
                break;

            case 16:
                this.drpStuff16.SelectedValue = cycleFoodId.ToString();
                this.txt_16.Text = ration.ToString();
                break;

            case 17:
                this.drpStuff17.SelectedValue = cycleFoodId.ToString();
                this.txt_17.Text = ration.ToString();
                break;

            case 18:
                this.drpStuff18.SelectedValue = cycleFoodId.ToString();
                this.txt_18.Text = ration.ToString();
                break;

            case 19:
                this.drpStuff19.SelectedValue = cycleFoodId.ToString();
                this.txt_19.Text = ration.ToString();
                break;

            case 20:
                this.drpStuff20.SelectedValue = cycleFoodId.ToString();
                this.txt_20.Text = ration.ToString();
                break;

            case 21:
                this.drpStuff21.SelectedValue = cycleFoodId.ToString();
                this.txt_21.Text = ration.ToString();
                break;

            case 22:
                this.drpStuff22.SelectedValue = cycleFoodId.ToString();
                this.txt_22.Text = ration.ToString();
                break;

            case 23:
                this.drpStuff23.SelectedValue = cycleFoodId.ToString();
                this.txt_23.Text = ration.ToString();
                break;

            case 24:
                this.drpStuff24.SelectedValue = cycleFoodId.ToString();
                this.txt_24.Text = ration.ToString();
                break;

            case 25:
                this.drpStuff25.SelectedValue = cycleFoodId.ToString();
                this.txt_25.Text = ration.ToString();
                break;

            case 26:
                this.drpStuff26.SelectedValue = cycleFoodId.ToString();
                this.txt_26.Text = ration.ToString();
                break;

            case 27:
                this.drpStuff27.SelectedValue = cycleFoodId.ToString();
                this.txt_27.Text = ration.ToString();
                break;

            case 28:
                this.drpStuff28.SelectedValue = cycleFoodId.ToString();
                this.txt_28.Text = ration.ToString();
                break;

            case 29:
                this.drpStuff29.SelectedValue = cycleFoodId.ToString();
                this.txt_29.Text = ration.ToString();
                break;

            case 30:
                this.drpStuff30.SelectedValue = cycleFoodId.ToString();
                this.txt_30.Text = ration.ToString();
                break;

            case 31:
                this.drpStuff31.SelectedValue = cycleFoodId.ToString();
                this.txt_31.Text = ration.ToString();
                break;
        }
    }
}