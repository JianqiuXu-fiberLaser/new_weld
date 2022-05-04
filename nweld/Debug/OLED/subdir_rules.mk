################################################################################
# Automatically-generated file. Do not edit!
################################################################################

SHELL = cmd.exe

# Each subdirectory must supply rules for building sources it contributes
OLED/oled.obj: ../OLED/oled.c $(GEN_OPTS) | $(GEN_HDRS)
	@echo 'Building file: $<'
	@echo 'Invoking: C2000 Compiler'
	"C:/ti/ccsv7/tools/compiler/ti-cgt-c2000_16.9.3.LTS/bin/cl2000" -v28 -ml -mt --include_path="D:/ccs/nweld" --include_path="C:/ti/ccsv7/tools/compiler/ti-cgt-c2000_16.9.3.LTS/include" --include_path="D:/ccs/nweld" --include_path="C:/tidcs/c28/DSP281x/v120/DSP281x_common/include" --include_path="C:/tidcs/c28/DSP281x/v120/DSP281x_headers/include" --advice:performance=all -g --c99 --diag_warning=225 --diag_wrap=off --display_error_number --preproc_with_compile --preproc_dependency="OLED/oled.d" --obj_directory="OLED" $(GEN_OPTS__FLAG) "$<"
	@echo 'Finished building: $<'
	@echo ' '

OLED/weld_oled.obj: ../OLED/weld_oled.c $(GEN_OPTS) | $(GEN_HDRS)
	@echo 'Building file: $<'
	@echo 'Invoking: C2000 Compiler'
	"C:/ti/ccsv7/tools/compiler/ti-cgt-c2000_16.9.3.LTS/bin/cl2000" -v28 -ml -mt --include_path="D:/ccs/nweld" --include_path="C:/ti/ccsv7/tools/compiler/ti-cgt-c2000_16.9.3.LTS/include" --include_path="D:/ccs/nweld" --include_path="C:/tidcs/c28/DSP281x/v120/DSP281x_common/include" --include_path="C:/tidcs/c28/DSP281x/v120/DSP281x_headers/include" --advice:performance=all -g --c99 --diag_warning=225 --diag_wrap=off --display_error_number --preproc_with_compile --preproc_dependency="OLED/weld_oled.d" --obj_directory="OLED" $(GEN_OPTS__FLAG) "$<"
	@echo 'Finished building: $<'
	@echo ' '


