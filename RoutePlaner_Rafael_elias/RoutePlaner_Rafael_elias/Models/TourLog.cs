using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RoutePlaner_Rafael_elias.Models
{
    public class TourLog
    {
        public int Id { get; set; }
        public int TourId { get; set; } 
        public DateTime Date { get; set; }
        public string Comment { get; set; }
        public int Difficulty { get; set; }
        public double TotalDistance { get; set; }
        public TimeSpan TotalTime { get; set; }
        public int Rating { get; set; }

        public virtual Tour Tour { get; set; } 
    }
    public class TourAddUtility 
    {
        string _name;
        string _startLocation;
        string _endLocation;
        string _description = " ";
        string _imagePath;
        float _distance;

        public string Name {
            get{
                return this._name;
            }
            set
            {
                if(this._name != value)
                {
                    this._name = value;
                }
            }
        }
        public string StartLocation {
            get 
            {
                return this._startLocation;
            }
            set 
            { 
                if(this._startLocation != value)
                {
                    this._startLocation = value;
                }
            }
        }
        public string EndLocation {
            get 
            {
                return this._endLocation;
            } 
            set
            {
                if(this._endLocation != value)
                {
                    this._endLocation = value;
                }
            } 
        }
        public string Description 
        {
            get
            {
                return this._description;
            }
            set
            {
                if (this._description != value)
                {
                    this._description = value;
                }
            }
        } 
        public string ImagePath { 
            get 
            {
                return this._imagePath; 
            }
            set
            {
                if (_imagePath != value)
                {
                    this._imagePath = value;
                }
            }
        }
        public float Distance {
            get 
            { 
                return this._distance; 
            }
            set 
            {
                if (_distance != value) 
                {
                    this._distance = value;
                }
            }
        }
    }
}
