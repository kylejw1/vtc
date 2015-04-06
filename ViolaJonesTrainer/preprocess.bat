find negative_images/ -name *.bmp > negatives.txt
find positive_images/ -name *.bmp > positives.txtran
gawk '{print $1 " 1 14 14 30 30" }' positives.txt > positives.tmp && mv positives.tmp positives.txt
opencv_createsamples.exe -info positives.txt -vec samples.vec -bg negatives.txt -w 30 -h 30 -num 54
