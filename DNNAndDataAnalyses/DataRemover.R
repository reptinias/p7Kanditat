#------- Libraries -------
library(tidyverse)
library(readbulk)
library(magrittr)
library(dplyr)

#------- Import and save raw Logged data --------
# Combine all logged data files into one data frame
dfP = readbulk::read_bulk('Data', sep=';', na.strings = 'none', stringsAsFactors=FALSE)

# In R there are two main ways to get a quick overview of the data
str(dfP)

summary(dfP)

#Save the imported files
save(dfP, file='logged_data_Raw.rda', compress=TRUE)

#------- Load Raw Data From rda --------

#Load
load("logged_data_Raw.rda")

# Remove everything except for the coordinate data
Data = dfP[ , -which(names(dfP) %in% c("Timestamp", "Framecount", "SessionID", "Email", "File", "fileFormatVersion..2"))]




Data <- sapply( Data, as.numeric )  


summary(Data)

cleanData <- na.omit(Data)

write.csv(cleanData,"C:/Users/kspar/PycharmProjects/GestureDNN/newCleanData.csv", row.names = FALSE)
