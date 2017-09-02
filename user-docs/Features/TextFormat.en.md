Formatting text
==========================

Text formatting in Sparkle Networks is based on blocks of text (titles, paragraphs, bullet lists). Blocks are delimited by **two line breaks**.  

Level 1 and level 2 titles
-----------------------------

To create a level 1 title, make a block that ends with a line containing many *equals* symbols. Level 2 titles looks the same but use *dash* symbols.

    This is a level 1 title
    =============================

    This is a level 2 title
    -----------------------------

In this document, the "Formatting text" line is a level 1 title. The line "Level 1 and level 2 titles" is a level 2 title.

You have other levels of titles. To use them, prefix your block with *sharp* symbols, as many as the level you want. 

    ### Level 3 title
    
    Some text under the level 3 title...
    
    ### Level 4 title
    
    Some text under the level 4 title...

Titles are useful to give structure to your documents. 

---

There is a horizontal rule you can also use (there is one just above this line). Make a block with 3 *dashes*.

    Some text...
    
    ---
    
    Some more text... 


Links
==============

Putting an url in the middle of your text will automatically make a link : http://bing.com/ .

Make sure to keep punctuation clear from your url by addind an extra space as separator.

You may desire to *name your link* to make it more friendly. There is a special syntax for that. Put the name of the link between square brackets `[]` and the URL between parenthesis `()`: 

    [link name](URL)

For example: [bing](http://bing.com/) or [DDG](http://duckduckgo.com/)

    For example: [bing](http://bing.com/) or [DDG](http://duckduckgo.com/)

Basic formatting
=================

Accentuate
-----------------------

There two levels of accentuation. Wrap your words between *single stars* or **double stars**.

    There two levels of accentuation. Wrap your words between *single stars* or **double stars**.

Break line in a paragraph
-----------------------

If I type a block of text 
with line breaks in it
it will not work. To create real line breaks inside your paragraphs,   
type two spaces before your line break.


    Si je saisie un bloc de texte 
    avec des passages à la
    ligne, cela n'a pas d'effet. Pour créer un vrai passage à la ligne dans un paragraphe, il faut que la ligne  
    se termine pas deux espaces.

Advanced formatting
=================

Bullet lists
-----------------------

* bullet lists are built by
- prefixing each line with a *dash* or a *star*. 
* it is a nice way to
* enumerate things.

- adding spaces before the symbol
  - will augment the level 
  - of indentation.
- Simple!

Monospace text
-----------------------

Prefixing lines of a block with 4 spaces (or a tab) will produce a *code block*. This document is full of this kind of block to highlight you how the syntax works.



