#!/bin/sh

DOCPATH=../build/tmpdoc
HTMLPATH=$DOCPATH/html
XMLPATH=$DOCPATH/xml
NAMESPACES=$(cd $HTMLPATH; ls namespace-*.html | sed -e 's/namespace-\(.*\)\.html/\1/g' | sort)

FILES="./api-title-page.html $HTMLPATH/index.html"
for n in $NAMESPACES
do
    FILES="$FILES $HTMLPATH/namespace-$n.html $(cd $XMLPATH; cat namespace-$n.xml | grep '<type name' | sed -e 's:.*name="\([^"]*\)".*:'$HTMLPATH'/type-\1.html:')"
done

htmldoc -t pdf --bodyfont serif --book --fontsize 9.0 --color $FILES > api-guide.pdf
