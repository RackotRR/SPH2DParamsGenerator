using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;

namespace SPH2DParamsGenerator
{
    public partial class SPH2DParamsGeneratorForm : Form
    {
        string LastFolderOpened = null;

        public SPH2DParamsGeneratorForm()
        {
            InitializeComponent();

            Setup();
            UpdateElements();
            Text = string.Format("Simulation properties generator v{0}.{1}", Version.Major, Version.Minor);
        }

        void Setup()
        {
            SetupAverageVelocity();
            SetupArtificialViscosity();
            SetupWavesGenerator();
            SetupBoundaries();
            SetupTimeIntegration();
            SetupTimeStep();
            SetupDensity();
            SetupInternalForce();
            SetupConsistency();
        }
        void UpdateElements()
        {
            UpdateArtificialViscosity();
            UpdateAverageVelocity();
            UpdateDynamicViscosity();
            UpdateWavesGenerator();
            UpdateTimeIntegration();
            UpdateDensity();
            UpdateConsistency();
            UpdateTimeStep();
        }

        void SetupSKFItems(ComboBox.ObjectCollection comboBoxItems)
        {
            comboBoxItems.Clear();
            comboBoxItems.Add("Cubic");
            comboBoxItems.Add("Gauss");
            comboBoxItems.Add("Qintic");
            comboBoxItems.Add("Desbrun");
        }

        void SetupArtificialViscosity()
        {
            SetupSKFItems(comboBox_ArtViscSKF.Items);
            comboBox_ArtViscSKF.SelectedIndex = skf.toIndex(skf.cubic);
            comboBox_ArtViscSKF.DropDownStyle = ComboBoxStyle.DropDownList;
        }
        void UpdateArtificialViscosity()
        {
            bool enabled = checkBox_EnableArtVisc.Checked;
            comboBox_ArtViscSKF.Enabled = enabled;
            textBox_ArtViscBulk.Enabled = enabled;
            textBox_ArtViscShear.Enabled = enabled;
        }

        void SetupAverageVelocity()
        {
            SetupSKFItems(comboBox_AvVelSKF.Items);
            comboBox_AvVelSKF.SelectedIndex = skf.toIndex(skf.cubic);
            comboBox_AvVelSKF.DropDownStyle = ComboBoxStyle.DropDownList;
        }
        void UpdateAverageVelocity()
        {
            bool enabled = checkBox_EnableAvVel.Checked;
            comboBox_AvVelSKF.Enabled = enabled;
            textBox_AvVelCoef.Enabled = enabled;
        }

        void UpdateDynamicViscosity()
        {
            textBox_DynViscValue.Enabled = checkBox_EnableDynVisc.Checked;
        }

        void SetupWavesGenerator()
        {
            comboBox_WavesGenTreat.Items.Clear();
            comboBox_WavesGenTreat.Items.Add("No waves");
            comboBox_WavesGenTreat.Items.Add("RZM");
            comboBox_WavesGenTreat.Items.Add("Dynamic");
            comboBox_WavesGenTreat.Items.Add("Impulse");
            comboBox_WavesGenTreat.Items.Add("Disappear wall");
            comboBox_WavesGenTreat.SelectedIndex = nwm.no_waves;
            comboBox_WavesGenTreat.DropDownStyle = ComboBoxStyle.DropDownList;
        }
        void UpdateWavesGenerator()
        {
            bool enabled = comboBox_WavesGenTreat.SelectedIndex != nwm.no_waves;
            bool disappear_wall_treatment = comboBox_WavesGenTreat.SelectedIndex == nwm.wall_disappear;
            comboBox_WavesGenTreat.Enabled = enabled;
            textBox_WavesGenLen.Enabled = enabled && !disappear_wall_treatment;
            textBox_WavesGenMagnitude.Enabled = enabled && !disappear_wall_treatment;
            textBox_WavesGenTimeWait.Enabled = enabled;
        }

        void SetupBoundaries()
        {
            comboBox_BoundaryTreat.Items.Clear();
            comboBox_BoundaryTreat.Items.Add("Dynamic");
            comboBox_BoundaryTreat.Items.Add("Repulsive");
            comboBox_BoundaryTreat.SelectedIndex = sbt.repulsive;
            comboBox_BoundaryTreat.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        void SetupTimeIntegration()
        {
            comboBox_TimeTreat.Items.Clear();
            comboBox_TimeTreat.Items.Add("Const: value");
            comboBox_TimeTreat.Items.Add("Const: estimate");
            comboBox_TimeTreat.Items.Add("Corrective");
            comboBox_TimeTreat.SelectedIndex = dt_method.const_CFL;
            comboBox_TimeTreat.DropDownStyle = ComboBoxStyle.DropDownList;
        }
        void UpdateTimeIntegration()
        {
            int index = comboBox_TimeTreat.SelectedIndex;
            textBox_TimeDT.Enabled = index == dt_method.const_value;
            textBox_TimeCFL.Enabled = index != dt_method.const_value;
        }

        void SetupDensity()
        {
            comboBox_DensityTreat.Items.Clear();
            comboBox_DensityTreat.Items.Add("Summation");
            comboBox_DensityTreat.Items.Add("Continuity");
            comboBox_DensityTreat.SelectedIndex = density_method.summation;
            comboBox_DensityTreat.DropDownStyle = ComboBoxStyle.DropDownList;

            SetupSKFItems(comboBox_DensitySKF.Items);
            comboBox_DensitySKF.SelectedIndex = skf.toIndex(skf.cubic);
            comboBox_DensitySKF.DropDownStyle = ComboBoxStyle.DropDownList;
        }
        void UpdateDensity()
        {
            bool summationTreatment = comboBox_DensityTreat.SelectedIndex == density_method.summation;
            checkBox_DensityNorm.Enabled = summationTreatment;
        }

        void SetupInternalForce()
        {
            comboBox_IntForceTreat.Items.Clear();
            comboBox_IntForceTreat.Items.Add("(pi + pj) / (\u03c1i * \u03c1j)");
            comboBox_IntForceTreat.Items.Add("pi/\u03c1j2 + pj/\u03c1j2");
            comboBox_IntForceTreat.SelectedIndex = 1;
            comboBox_IntForceTreat.DropDownStyle = ComboBoxStyle.DropDownList;

            SetupSKFItems(comboBox_IntForceSKF.Items);
            comboBox_IntForceSKF.SelectedIndex = skf.toIndex(skf.cubic);
            comboBox_IntForceSKF.DropDownStyle = ComboBoxStyle.DropDownList;

            comboBox_IntForceSoundVelMethod.Items.Clear();
            comboBox_IntForceSoundVelMethod.Items.Add("Dam break");
            comboBox_IntForceSoundVelMethod.Items.Add("Specific");
            comboBox_IntForceSoundVelMethod.SelectedIndex = eos_sound_vel_method.dam_break;
            comboBox_IntForceSoundVelMethod.DropDownStyle = ComboBoxStyle.DropDownList;
        }
        void UpdateInternalForce()
        {
            if (comboBox_IntForceSoundVelMethod.SelectedIndex == eos_sound_vel_method.dam_break)
            {
                label_IntForceSoundVelCoef.Text = "Sound velocity coef.";
            }
            else
            {
                label_IntForceSoundVelCoef.Text = "Sound velocity";
            }
        }

        void SetupTimeStep()
        {
            comboBox_StepTreatment.Items.Clear();
            comboBox_StepTreatment.Items.Add("Steps");
            comboBox_StepTreatment.Items.Add("Time");
            comboBox_StepTreatment.SelectedIndex = 0;
            comboBox_StepTreatment.DropDownStyle = ComboBoxStyle.DropDownList;
        }
        void UpdateTimeStep()
        {
            bool use_steps = comboBox_StepTreatment.SelectedIndex == stepping_treatment.step;
            if (use_steps)
            {
                label_StepSave.Text = "Save step";
                label_StepDump.Text = "Dump step";
            }
            else
            {
                label_StepSave.Text = "Save time";
                label_StepDump.Text = "Dump time";
            }

            textBox_StepDump.Enabled = checkBox_StepEnableDump.Checked;
            textBox_StepEstimate.Enabled = checkBox_StepEnableEstimate.Checked;
        }

        void SetupConsistency()
        {
            comboBox_ConsistencyTreat.Items.Clear();
            comboBox_ConsistencyTreat.Items.Add("Print");
            comboBox_ConsistencyTreat.Items.Add("Stop");
            comboBox_ConsistencyTreat.Items.Add("Fix");
            comboBox_ConsistencyTreat.SelectedIndex = 1;
            comboBox_ConsistencyTreat.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        void UpdateConsistency()
        {
            bool enabled = checkBox_ConsistencyCheck.Checked;
            comboBox_ConsistencyTreat.Enabled = enabled;
            textBox_ConsistencyStep.Enabled = enabled;
        }

        int? NullParseInt(string str)
        {
            int val;
            if (int.TryParse(str, out val))
            {
                return val;
            }
            else
            {
                return null;
            }
        }
        float? NullParseFloat(string str)
        {
            float val;
            if (float.TryParse(str, out val))
            {
                return val;
            }
            else
            {
                return null;
            }
        }
        int ParseInt(string str, string field)
        {
            int val;
            if (int.TryParse(str, out val))
            {
                return val;
            }
            else
            {
                throw new Exception("Can't parse mandatory field '" + field + "'.");
            }
        }
        float ParseFloat(string str, string field)
        {
            float val;
            if (float.TryParse(str, out val))
            {
                return val;
            }
            else
            {
                throw new Exception("Can't parse mandatory field '" + field + "'.");
            }
        }

        void FillInArtificialViscosity(ExperimentParams experimentParams)
        {
            experimentParams.artificial_viscosity = checkBox_EnableArtVisc.Checked;
            if (experimentParams.artificial_viscosity.Value)
            {
                experimentParams.artificial_bulk_visc = ParseFloat(textBox_ArtViscBulk.Text, "artifical_bulk_visc");
                experimentParams.artificial_shear_visc = ParseFloat(textBox_ArtViscShear.Text, "artificial_shear_visc");
                experimentParams.artificial_viscosity_skf = skf.fromIndex(comboBox_ArtViscSKF.SelectedIndex);
            }
        }
        void FillInAverageVelocity(ExperimentParams experimentParams)
        {
            experimentParams.average_velocity = checkBox_EnableAvVel.Checked;
            if (experimentParams.average_velocity.Value)
            {
                experimentParams.average_velocity_coef = ParseFloat(textBox_AvVelCoef.Text, "average_velocity_coef");
                experimentParams.average_velocity_skf = skf.fromIndex(comboBox_AvVelSKF.SelectedIndex);
            }
        }
        void FillInDensity(ExperimentParams experimentParams)
        {
            experimentParams.density_treatment = comboBox_DensityTreat.SelectedIndex;
            experimentParams.density_skf = skf.fromIndex(comboBox_DensitySKF.SelectedIndex);
            if (checkBox_DensityNorm.Checked)
            {
                experimentParams.density_normalization = density_normalization.basic;
            }
        }
        void FillInInternalForce(ExperimentParams experimentParams)
        {
            experimentParams.intf_hsml_coef = NullParseFloat(textBox_IntForceHsml.Text);
            experimentParams.intf_skf = skf.fromIndex(comboBox_IntForceSKF.SelectedIndex);
            experimentParams.intf_sph_approximation = pa_sph.fromIndex(comboBox_IntForceTreat.SelectedIndex);
            experimentParams.eos_sound_vel_method = comboBox_IntForceSoundVelMethod.SelectedIndex;
            if (experimentParams.eos_sound_vel_method == eos_sound_vel_method.dam_break)
            {
                experimentParams.eos_sound_vel_coef = ParseFloat(textBox_IntForceSoundVel.Text, "eos_sound_vel_coef");
            }
            else
            {
                experimentParams.eos_sound_vel = ParseFloat(textBox_IntForceSoundVel.Text, "eos_sound_vel");
            }
        }
        void FillInWavesGenerator(ExperimentParams experimentParams)
        {
            experimentParams.boundary_treatment = comboBox_BoundaryTreat.SelectedIndex;
            experimentParams.nwm = comboBox_WavesGenTreat.SelectedIndex;
            if (experimentParams.nwm.Value != nwm.no_waves)
            {
                experimentParams.nwm_wait = ParseFloat(textBox_WavesGenTimeWait.Text, "nwm_wait");
                experimentParams.nwm_wave_magnitude = ParseFloat(textBox_WavesGenMagnitude.Text, "nwm_wave_magnitude");
                experimentParams.nwm_wave_length = ParseFloat(textBox_WavesGenLen.Text, "nwm_wave_length");
            }
        }
        void FillInTimeIntegration(ExperimentParams experimentParams)
        {
            experimentParams.simulation_time = ParseFloat(textBox_TimeSim.Text, "simulation_time");
            experimentParams.dt_correction_method = comboBox_TimeTreat.SelectedIndex;
            if (experimentParams.dt_correction_method == dt_method.const_value)
            {
                experimentParams.dt = ParseFloat(textBox_TimeDT.Text, "dt");
            }
            else
            {
                experimentParams.CFL_coef = ParseFloat(textBox_TimeCFL.Text, "CFL_coef");
            }
        }
        void FillInTimeStep(ExperimentParams experimentParams)
        {
            bool enable_estimate = checkBox_StepEnableEstimate.Checked;
            bool enable_dump = checkBox_StepEnableDump.Checked;
            int step_treatment = comboBox_StepTreatment.SelectedIndex;

            experimentParams.step_treatment = step_treatment;

            if (enable_estimate)
            {
                experimentParams.step_time_estimate = NullParseInt(textBox_StepEstimate.Text);
            }

            if (step_treatment == stepping_treatment.step)
            {
                experimentParams.save_step = ParseInt(textBox_StepSave.Text, "save_step");
                if (enable_dump)
                {
                    experimentParams.dump_step = NullParseInt(textBox_StepDump.Text);
                }
            }
            else
            {
                experimentParams.save_time = ParseFloat(textBox_StepSave.Text, "save_time");
                if (enable_dump)
                {
                    experimentParams.dump_time = NullParseFloat(textBox_StepDump.Text);
                }
            }
        }
        void FillInViscosity(ExperimentParams experimentParams)
        {
            experimentParams.visc = checkBox_EnableDynVisc.Checked;
            if (experimentParams.visc.Value)
            {
                experimentParams.visc_coef = NullParseFloat(textBox_DynViscValue.Text);
            }
        }
        void FillInConsistency(ExperimentParams experimentParams)
        {
            experimentParams.consistency_check = checkBox_ConsistencyCheck.Checked;
            if (experimentParams.consistency_check.Value)
            {
                experimentParams.consistency_treatment = comboBox_ConsistencyTreat.SelectedIndex;
                experimentParams.consistency_check_step = NullParseInt(textBox_ConsistencyStep.Text);
            }
        }
        void FillInExtra(ExperimentParams experimentParams)
        {
            experimentParams.local_threads = NullParseInt(textBox_ExtraLocalThreads.Text);
            experimentParams.max_neighbours = NullParseInt(textBox_ExtraMaxNeighbours.Text);
        }
        string FindExperimentDirectory(string directory)
        {
            if (!Directory.Exists(directory))
            {
                return directory;
            }

            int index = directory.LastIndexOf('_');
            if (index + 3 == directory.Length)
            {
                string numberStr = directory.Substring(index + 1);
                int number;
                if (int.TryParse(numberStr, out number))
                {
                    string nextExperimentName = directory.Substring(0, index + 1) + (number + 1).ToString("D2");
                    return FindExperimentDirectory(nextExperimentName);
                }
            }

            return FindExperimentDirectory(directory + "_01");
        }
        ExperimentParams GenerateExperimentParams()
        {
            var experimentParams = new ExperimentParams();

            FillInArtificialViscosity(experimentParams);
            FillInAverageVelocity(experimentParams);
            FillInDensity(experimentParams);
            FillInInternalForce(experimentParams);
            FillInWavesGenerator(experimentParams);
            FillInTimeStep(experimentParams);
            FillInTimeIntegration(experimentParams);
            FillInViscosity(experimentParams);
            FillInConsistency(experimentParams);
            FillInExtra(experimentParams);

            experimentParams.params_generator_version_major = Version.Major;
            experimentParams.params_generator_version_minor = Version.Minor;

            return experimentParams;
        }
        void GenerateProject(string directory, ExperimentParams experiment_params)
        {
            string dir = Path.Combine(directory, textBox_ExperimentName.Text);
            if (Directory.Exists(dir))
            {
                DialogResult result = MessageBox.Show(
                    "Experiment directory already exists. Override experiment?", 
                    "Warning", 
                    MessageBoxButtons.YesNoCancel, 
                    MessageBoxIcon.Warning);

                if (result == DialogResult.Cancel)
                {
                    return;
                }
                else if (result == DialogResult.Yes)
                {
                    Directory.Delete(dir, true);
                }
                else if (result == DialogResult.No)
                {
                    dir = FindExperimentDirectory(dir);
                    textBox_ExperimentName.Text = Path.GetFileName(dir);
                }
            }
            Directory.CreateDirectory(dir);

            string path = Path.Combine(dir, "ModelParams.json");
            using (var stream = File.OpenWrite(path))
            {
                var options = new JsonSerializerOptions 
                { 
                    WriteIndented = true, 
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull 
                };
                JsonSerializer.Serialize(stream, experiment_params, options);
            }
        }

        void LoadArtificialViscosity(ExperimentParams experimentParams)
        {
            checkBox_EnableArtVisc.Checked = experimentParams.artificial_viscosity ?? false;
            comboBox_ArtViscSKF.SelectedIndex = skf.toIndex(experimentParams.artificial_viscosity_skf ?? skf.cubic);

            textBox_ArtViscBulk.Text = experimentParams.artificial_bulk_visc?.ToString();
            textBox_ArtViscShear.Text = experimentParams.artificial_shear_visc?.ToString();
        }

        void LoadAverageVelocity(ExperimentParams experimentParams)
        {
            checkBox_EnableAvVel.Checked = experimentParams.average_velocity ?? false;

            comboBox_AvVelSKF.SelectedIndex = skf.toIndex(experimentParams.average_velocity_skf ?? skf.cubic);
            textBox_AvVelCoef.Text = experimentParams.average_velocity_coef?.ToString();
        }
        void LoadInternalForce(ExperimentParams experimentParams)
        {
            comboBox_IntForceSKF.SelectedIndex = skf.toIndex(experimentParams.intf_skf ?? skf.cubic);
            comboBox_IntForceTreat.SelectedIndex = pa_sph.toIndex(experimentParams.intf_sph_approximation ?? 2);
            textBox_IntForceHsml.Text = (experimentParams.intf_hsml_coef ?? 1).ToString();
            comboBox_IntForceSoundVelMethod.SelectedIndex = (experimentParams.eos_sound_vel_method);
            if (experimentParams.eos_sound_vel_method == eos_sound_vel_method.dam_break)
            {
                textBox_IntForceSoundVel.Text = experimentParams.eos_sound_vel_coef?.ToString();
            }
            else
            {
                textBox_IntForceSoundVel.Text = experimentParams.eos_sound_vel?.ToString();
            }
        }
        void LoadDensity(ExperimentParams experimentParams)
        {
            comboBox_DensityTreat.SelectedIndex = experimentParams.density_treatment ?? density_method.continuity;
            comboBox_DensitySKF.SelectedIndex = skf.toIndex(experimentParams.density_skf ?? skf.cubic);
            if (experimentParams.density_normalization is int norm_density)
            {
                checkBox_DensityNorm.Checked = norm_density == 1;
            }
            else
            {
                checkBox_DensityNorm.Checked = false;
            }
        }
        void LoadWavesGenerator(ExperimentParams experimentParams)
        {
            comboBox_WavesGenTreat.SelectedIndex = (experimentParams.nwm ?? nwm.no_waves);
            textBox_WavesGenLen.Text = experimentParams.nwm_wave_length?.ToString();
            textBox_WavesGenMagnitude.Text = experimentParams.nwm_wave_magnitude?.ToString();
            textBox_WavesGenTimeWait.Text = experimentParams.nwm_wait?.ToString();
            comboBox_BoundaryTreat.SelectedIndex = experimentParams.boundary_treatment;
        }
        void LoadDynamicViscosity(ExperimentParams experimentParams)
        {
            checkBox_EnableDynVisc.Checked = experimentParams.visc ?? false;
            textBox_DynViscValue.Text = experimentParams.visc_coef?.ToString();
        }
        void LoadTimeIntegration(ExperimentParams experimentParams)
        {
            comboBox_TimeTreat.SelectedIndex = (experimentParams.dt_correction_method ?? dt_method.dynamic);
            textBox_TimeCFL.Text = experimentParams.CFL_coef?.ToString();
            textBox_TimeDT.Text = experimentParams.dt?.ToString();
            textBox_TimeSim.Text = experimentParams.simulation_time.ToString();
        }
        void LoadTimeStep(ExperimentParams experimentParams)
        {
            comboBox_StepTreatment.SelectedIndex = experimentParams.step_treatment ?? stepping_treatment.step;
            if (experimentParams.step_treatment is int step_treatment)
            {
                if (step_treatment == stepping_treatment.step)
                {
                    textBox_StepSave.Text = experimentParams.save_step?.ToString();
                    textBox_StepDump.Text = experimentParams.dump_step?.ToString();
                }
                else
                {
                    textBox_StepSave.Text = experimentParams.save_time?.ToString();
                    textBox_StepDump.Text = experimentParams.dump_time?.ToString();
                }
            }
            textBox_StepEstimate.Text = experimentParams.step_time_estimate?.ToString();
        }
        void LoadConsistency(ExperimentParams experimentParams)
        {
            checkBox_ConsistencyCheck.Checked = experimentParams.consistency_check ?? true;
            textBox_ConsistencyStep.Text = (experimentParams.consistency_check_step ?? 1).ToString();
            comboBox_ConsistencyTreat.SelectedIndex = experimentParams.consistency_treatment ?? consistency_treatment.stop;
        }
        void LoadExtra(ExperimentParams experimentParams)
        {
            textBox_ExtraLocalThreads.Text = experimentParams.local_threads?.ToString();
            textBox_ExtraMaxNeighbours.Text = experimentParams.max_neighbours?.ToString();
        }
        void LoadTemplate(string filename)
        {
            using (var stream = File.OpenRead(filename))
            {
                var experimentParams = JsonSerializer.Deserialize<ExperimentParams>(stream);
                if (experimentParams != null)
                {
                    bool majorLess = experimentParams.params_generator_version_major < Version.Major;
                    bool majorEqual = experimentParams.params_generator_version_major == Version.Major;
                    bool minorLess = experimentParams.params_generator_version_minor < Version.Minor;
                    if (majorLess || (majorEqual && minorLess))
                    {
                        DialogResult result = MessageBox.Show(
                            "Expected higher params version. Try to parse parameters anyway?",
                            "Warning",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Warning);

                        if (result == DialogResult.No)
                        {
                            return;
                        }
                    }

                    LoadArtificialViscosity(experimentParams);
                    LoadAverageVelocity(experimentParams);
                    LoadDensity(experimentParams);
                    LoadInternalForce(experimentParams);
                    LoadWavesGenerator(experimentParams);
                    LoadDynamicViscosity(experimentParams);
                    LoadTimeIntegration(experimentParams);
                    LoadTimeStep(experimentParams);
                    LoadConsistency(experimentParams);
                    LoadExtra(experimentParams);
                }
            }
        }

        bool CheckNotImplemented(ExperimentParams experimentParams)
        {
            string text = string.Empty;

            if (experimentParams.density_normalization == density_normalization.basic)
            {
                text = "density normalization";
            }

            if (experimentParams.artificial_viscosity_skf == skf.qintic ||
                experimentParams.average_velocity_skf == skf.qintic ||
                experimentParams.density_skf == skf.qintic ||
                experimentParams.intf_skf == skf.qintic)
            {
                text = "qintic skf";
            }

            if (experimentParams.dt_correction_method == dt_method.dynamic)
            {
                text = "dynamic dt correction";
            }

            if (experimentParams.step_treatment == stepping_treatment.time && 
                experimentParams.dt_correction_method != dt_method.const_value)
            {
                text = "time step treatment with CFL";
            }

            if (experimentParams.nwm == nwm.relaxation_zone ||
                experimentParams.nwm == nwm.impulse)
            {
                text = "waves generator";
            }

            if (text == string.Empty)
            {
                return true;
            }
            else
            {
                MessageBox.Show(string.Format("Feature '{0}' is not implemented. Use another options.", text), "Not implemented error");
                return false;
            }
        }

        private void checkBox_EnableArtVisc_CheckedChanged(object sender, EventArgs e)
        {
            UpdateArtificialViscosity();
        }

        private void checkBox_EnableAvVel_CheckedChanged(object sender, EventArgs e)
        {
            UpdateAverageVelocity();
        }

        private void checkBox_EnableDynVisc_CheckedChanged(object sender, EventArgs e)
        {
            UpdateDynamicViscosity();
        }

        private void checkBox_EnableWavesGen_CheckedChanged(object sender, EventArgs e)
        {
            UpdateWavesGenerator();
        }

        private void comboBox_TimeTreat_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateTimeIntegration();
        }

        private void comboBox_DensityTreat_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateDensity();
        }

        private void button_GenerateProject_Click(object sender, EventArgs e)
        {
            ExperimentParams experiment_params;
            try
            {
                experiment_params = GenerateExperimentParams();
                if (CheckNotImplemented(experiment_params) == false) return;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Params generation failed: " + ex.Message);
                return;
            }

            using (var dialog = new CommonOpenFileDialog())
            {
                dialog.InitialDirectory = LastFolderOpened ?? Directory.GetCurrentDirectory();
                dialog.IsFolderPicker = true;
                var result = dialog.ShowDialog();
                if (result == CommonFileDialogResult.Ok && !string.IsNullOrWhiteSpace(dialog.FileName))
                {
                    LastFolderOpened = dialog.FileName;
                    try
                    {
                        GenerateProject(dialog.FileName, experiment_params);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error happened: " + ex.Message);
                    }
                }
            }
        }

        private void button_OpenAsTemplate_Click(object sender, EventArgs e)
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.InitialDirectory = LastFolderOpened ?? Directory.GetCurrentDirectory();
                dialog.Filter = "Params file (*.json)|*.json|All files (*.*)|*.*";
                var result = dialog.ShowDialog();
                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.FileName))
                {
                    LastFolderOpened = Path.GetDirectoryName(dialog.FileName);
                    try
                    {
                        LoadTemplate(dialog.FileName);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error happened: " + ex.Message);
                    }
                }
            }
        }

        private void checkBox_ConsistencyCheck_CheckedChanged(object sender, EventArgs e)
        {
            UpdateConsistency();
        }

        private void comboBox_IntForceSoundVelMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateInternalForce();
        }

        private void comboBox_WavesGenTreat_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateWavesGenerator();
        }

        private void comboBox_StepTreatment_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateTimeStep();
        }

        private void checkBox_StepEnableEstimate_CheckedChanged(object sender, EventArgs e)
        {
            UpdateTimeStep();
        }

        private void checkBox_StepEnableDump_CheckedChanged(object sender, EventArgs e)
        {
            UpdateTimeStep();
        }
    }
}
