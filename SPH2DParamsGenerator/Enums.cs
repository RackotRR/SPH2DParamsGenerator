using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace SPH2DParamsGenerator
{
    class pa_sph
    {
        public static int toIndex(uint pa_sph) => (int)pa_sph - 1;
        public static uint fromIndex(int index) => (uint)(index + 1);
    }

    class skf // smoothing kernel function
    {
        public static uint cubic => 1;
        public static uint gauss => 2;
        public static uint quintic => 3;
        public static uint desbrun => 4;

        public static int toIndex(uint skf) => (int)skf - 1;
        public static uint fromIndex(int index) => (uint)(index + 1);
    }

    class sbt // solid boundary treatment
    {
        public static int dynamic => 0;
        public static int repulsive => 1;
    }

    class nwm // numerical waves maker
    {
        public static uint no_waves => 0;
        public static uint relaxation_zone => 1;
        public static uint dynamic => 2;
        public static uint impulse => 3;
        public static uint wall_disappear => 4;

        public static int toIndex(uint nwm) => (int)nwm - 1;
        public static uint fromIndex(int index) => (uint)(index + 1);
    }

    class dt_method
    {
        public static int const_value => 0;
        public static int const_CFL => 1;
        public static int dynamic => 2;
    }

    class stepping_treatment
    {
        public static int step => 0;
        public static int time => 1;
    }

    class eos_sound_vel_method
    {
        public static int dam_break => 0;
        public static int specific => 1;
    }

    class particle_generator
    {
        public static int script => 0;
        public static int pic_gec => 1;
    }

    class density_method
    {
        public static int summation => 0;
        public static int continuity => 1;
    }
}
