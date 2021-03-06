//
//  RichTextFactoryTest.java
//
//  Lunar Unity Mobile Console
//  https://github.com/SpaceMadness/lunar-unity-console
//
//  Copyright 2015-2020 Alex Lementuev, SpaceMadness.
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//


package spacemadness.com.lunarconsole.console;

import android.text.SpannableString;
import android.text.Spanned;
import android.text.style.CharacterStyle;
import android.text.style.StyleSpan;

import org.junit.Before;
import org.junit.Test;

import static android.support.test.InstrumentationRegistry.getContext;
import static org.junit.Assert.assertEquals;
import static org.junit.Assert.assertNull;
import static spacemadness.com.lunarconsole.console.DefaultRichTextFactory.Span;

public class RichTextFactoryTest {
    private StyleSpan bold;
    private StyleSpan italic;
    private StyleSpan boldItalic;

    private DefaultRichTextFactory richTextFactory;
    private ColorFactory colorFactory;

    @Before
    public void setup() {
        colorFactory = new DefaultColorFactory(getContext());
        richTextFactory = new DefaultRichTextFactory(colorFactory);

        // need to use exact styles: otherwise SpannedString equality fails
        bold = richTextFactory.bold;
        italic = richTextFactory.italic;
        boldItalic = richTextFactory.boldItalic;
    }

    @Test
    public void testNullString() {
        CharSequence actual = fromRichText(null);
        assertNull(actual);
    }

    @Test
    public void testEmptyString() {
        CharSequence expected = "";
        CharSequence actual = fromRichText("");
        assertEquals(expected, actual);
    }

    @Test
    public void testNoRichTags() {
        CharSequence expected = "This is text.";
        CharSequence actual = fromRichText("This is text.");
        assertEquals(expected, actual);
    }

    @Test
    public void testBoldTags1() {
        CharSequence expected = createSpanned("This is text.", new Span(bold, 5, 2));
        CharSequence actual = fromRichText("This <b>is</b> text.");
        assertEquals(expected, actual);
    }

    @Test
    public void testBoldTags2() {
        CharSequence expected = createSpanned("This is text.", new Span(bold, 0, 7));
        CharSequence actual = fromRichText("<b>This is</b> text.");
        assertEquals(expected, actual);
    }

    @Test
    public void testBoldTags3() {
        CharSequence expected = createSpanned("This is text.", new Span(bold, 8, 5));
        CharSequence actual = fromRichText("This is <b>text.</b>");
        assertEquals(expected, actual);
    }

    @Test
    public void testBoldTags4() {
        CharSequence expected = "This is text.";
        CharSequence actual = fromRichText("This is <b></b>text.");
        assertEquals(expected, actual);
    }

    @Test
    public void testBoldTags5() {
        CharSequence expected = "This is text.";
        CharSequence actual = fromRichText("This is <b><b></b></b>text.");
        assertEquals(expected, actual);
    }

    @Test
    public void testItalicTags1() {
        CharSequence expected = createSpanned("This is text.", new Span(italic, 5, 2));
        CharSequence actual = fromRichText("This <i>is</i> text.");
        assertEquals(expected, actual);
    }

    @Test
    public void testItalicTags2() {
        CharSequence expected = createSpanned("This is text.", new Span(italic, 0, 7));
        CharSequence actual = fromRichText("<i>This is</i> text.");
        assertEquals(expected, actual);
    }

    @Test
    public void testItalicTags3() {
        CharSequence expected = createSpanned("This is text.", new Span(italic, 8, 5));
        CharSequence actual = fromRichText("This is <i>text.</i>");
        assertEquals(expected, actual);
    }

    @Test
    public void testItalicTags4() {
        CharSequence expected = "This is text.";
        CharSequence actual = fromRichText("This is <i></i>text.");
        assertEquals(expected, actual);
    }

    @Test
    public void testItalicTags5() {
        CharSequence expected = "This is text.";
        CharSequence actual = fromRichText("This is <i><i></i></i>text.");
        assertEquals(expected, actual);
    }

    @Test
    public void testColorTags1() {
        CharSequence expected = createSpanned("This is text.", new Span(createCharacterStyle("red"), 5, 2));
        CharSequence actual = fromRichText("This <color=red>is</color> text.");
        assertEquals(expected, actual);
    }

    @Test
    public void testColorTags2() {
        CharSequence expected = createSpanned("This is text.", new Span(createCharacterStyle("red"), 0, 7));
        CharSequence actual = fromRichText("<color=red>This is</color> text.");
        assertEquals(expected, actual);
    }

    @Test
    public void testColorTags3() {
        CharSequence expected = createSpanned("This is text.", new Span(createCharacterStyle("red"), 8, 5));
        CharSequence actual = fromRichText("This is <color=red>text.</color>");
        assertEquals(expected, actual);
    }

    @Test
    public void testColorTags4() {
        CharSequence expected = "This is text.";
        CharSequence actual = fromRichText("This is <color=red></color>text.");
        assertEquals(expected, actual);
    }

    @Test
    public void testColorTags5() {
        CharSequence expected = "This is text.";
        CharSequence actual = fromRichText("This is <color=red><color=red></color></color>text.");
        assertEquals(expected, actual);
    }

    @Test
    public void testMultipleTags1() {
        Span[] spans = {
                new Span(bold, 5, 2),
                new Span(createCharacterStyle("red"), 5, 2)
        };
        CharSequence expected = createSpanned("This is text.", spans);
        CharSequence actual = fromRichText("This <color=red><b>is</b></color> text.");
        assertEquals(expected, actual);
    }

    @Test
    public void testMultipleTags2() {
        Span[] spans = {
                new Span(bold, 12, 4),
                new Span(createCharacterStyle("red"), 8, 19)
        };
        CharSequence expected = createSpanned("This is red bold attributed text.", spans);
        CharSequence actual = fromRichText("This is <color=red>red <b>bold</b> attributed</color> text.");
        assertEquals(expected, actual);
    }

    @Test
    public void testMultipleTags3() {
        Span[] spans = {
                new Span(boldItalic, 17, 3),
                new Span(bold, 12, 11),
                new Span(createCharacterStyle("red"), 8, 26)
        };
        CharSequence expected = createSpanned("This is red bold italic attributed text.", spans);
        CharSequence actual = fromRichText("This is <color=red>red <b>bold <i>ita</i>lic</b> attributed</color> text.");
        assertEquals(expected, actual);
    }

    @Test
    public void testMalformedTags1() {
        CharSequence expected = "This is text.";
        CharSequence actual = fromRichText("This <b>is text.");
        assertEquals(expected, actual);
    }

    @Test
    public void testMalformedTags2() {
        CharSequence expected = "This is text.";
        CharSequence actual = fromRichText("This <b>is<b> text.");
        assertEquals(expected, actual);
    }

    @Test
    public void testMalformedTags3() {
        CharSequence expected = "This is text.";
        CharSequence actual = fromRichText("This <b>is</i> text.");
        assertEquals(expected, actual);
    }

    @Test
    public void testMalformedTags4() {
        CharSequence expected = "This is malformed text.";
        CharSequence actual = fromRichText("This <b>is <b>malformed</b> text.");
        assertEquals(expected, actual);
    }

    private CharSequence fromRichText(String text) {
        return richTextFactory.createRichText(text);
    }

    private CharSequence createSpanned(String text, Span... spans) {
        SpannableString string = new SpannableString(text);
        for (Span tag : spans) {
            string.setSpan(tag.style, tag.startIndex, tag.startIndex + tag.length, Spanned.SPAN_EXCLUSIVE_EXCLUSIVE);
        }
        return string;
    }

    private CharacterStyle createCharacterStyle(String value) {
        // we need to re-use the same style objects (otherwise SpannedString equality fails)
        return richTextFactory.styleFromColorValue(value);
    }
}
